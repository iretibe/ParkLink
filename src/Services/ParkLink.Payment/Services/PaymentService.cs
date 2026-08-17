using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ParkLink.Payment.Data;
using ParkLink.Payment.Dtos;
using ParkLink.Payment.Dtos.Paystack;
using ParkLink.Payment.Enums;
using ParkLink.Payment.Messages;
using ParkLink.Payment.Models;
using ParkLink.Payment.Providers;
using ParkLink.SharedKernel.Events.Payment;
using ParkLink.SharedKernel.Pagination;
using System.Text.Json;

namespace ParkLink.Payment.Services
{
    public class PaymentService : IPaymentService
    {
        private const int MaxPageSize = 100;

        private readonly PaymentContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IPaymentProviderResolver _providerResolver;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(PaymentContext context,
            IPublishEndpoint publishEndpoint,
            IPaymentProviderResolver providerResolver,
            ILogger<PaymentService> logger)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
            _providerResolver = providerResolver;
            _logger = logger;
        }

        public async Task<PaymentDto> CreatePaymentAsync(string userId, 
            CreatePaymentRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);

            if (request.Amount <= 0)
            {
                throw new ArgumentException(
                    "Payment amount must be greater than zero.");
            }

            var existing = await _context.Payments
                .FirstOrDefaultAsync(x => 
                    x.ReservationId == request.ReservationId, cancellationToken);

            if (existing != null)
            {
                return MapToDto(existing);
            }

            var payment = new Models.Payment
            {
                Id = Guid.NewGuid(),
                ReservationId = request.ReservationId,
                ReservationNumber = request.ReservationNumber,
                UserId = userId,
                VehicleId = request.VehicleId,
                Amount = request.Amount,
                CurrencyCode = request.CurrencyCode.ToUpperInvariant(),
                Method = request.Method,
                Status = PaymentStatus.Pending,
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.Payments.Add(payment);

            await _context.SaveChangesAsync(cancellationToken);

            var provider = _providerResolver.Resolve(request.Method);

            var providerRequest = new Dtos.Providers.PaymentProviderRequest(
                payment.Id,
                payment.ReservationId,
                payment.ReservationNumber,
                payment.UserId,
                payment.Amount,
                payment.CurrencyCode,
                request.CustomerEmail,
                request.CallbackUrl
            );

            var providerResult = await provider.InitializePaymentAsync(
                providerRequest, cancellationToken);

            if (!providerResult.Success)
            {
                payment.Status = PaymentStatus.Failed;
                payment.FailureReason = providerResult.FailureReason;
                payment.FailedAtUtc = DateTime.UtcNow;
                payment.UpdatedAtUtc = DateTime.UtcNow;

                AddTransaction(
                    payment,
                    PaymentTransactionType.Payment,
                    PaymentStatus.Failed,
                    payment.Amount,
                    providerResult.ProviderReference,
                    providerResult.FailureReason
                );

                try
                {
                    await _context.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateException)
                {
                    var existingPayment = await _context.Payments
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => 
                            x.ReservationId == request.ReservationId, cancellationToken);

                    if (existingPayment != null)
                    {
                        return MapToDto(existingPayment);
                    }

                    throw;
                }

                await _publishEndpoint.Publish(
                    new PaymentFailedIntegrationEvent(
                        payment.Id,
                        payment.ReservationId,
                        payment.ReservationNumber,
                        payment.UserId,
                        payment.VehicleId,
                        payment.Amount,
                        payment.CurrencyCode,
                        payment.PaymentReference,
                        payment.FailureReason ?? "Payment initialization failed.",
                        payment.FailedAtUtc.Value),
                    cancellationToken
                );

                return MapToDto(payment);
            }

            payment.Provider = provider.Name;
            payment.ProviderReference = providerResult.ProviderReference;
            payment.PaymentReference = providerResult.PaymentReference;
            //payment.Status = providerResult.RequiresAction
            //    ? PaymentStatus.Processing
            //    : PaymentStatus.Authorized;
            payment.Status = PaymentStatus.Processing;
            payment.AuthorizationUrl = providerResult.AuthorizationUrl;
            payment.AuthorizedAtUtc = payment.Status == PaymentStatus.Authorized
                ? DateTime.UtcNow
                : null;

            AddTransaction(
                payment,
                PaymentTransactionType.Payment,
                payment.Status,
                payment.Amount,
                providerResult.ProviderReference,
                null
            );

            await _context.SaveChangesAsync(cancellationToken);

            if (payment.Status == PaymentStatus.Authorized)
            {
                await _publishEndpoint.Publish(
                    new PaymentAuthorizedIntegrationEvent(
                        payment.Id,
                        payment.ReservationId,
                        payment.ReservationNumber,
                        payment.UserId,
                        payment.VehicleId,
                        payment.PaymentReference!,
                        payment.Amount,
                        payment.CurrencyCode,
                        payment.AuthorizedAtUtc!.Value),
                    cancellationToken
                );
            }

            return MapToDto(payment);
        }

        public async Task<PaymentDto?> GetPaymentByIdAsync(Guid paymentId,
            string userId, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);

            var payment = await _context.Payments
                .AsNoTracking()
                .FirstOrDefaultAsync(x => 
                    x.Id == paymentId && x.UserId == userId, cancellationToken);

            return payment == null ? null : MapToDto(payment);
        }

        public async Task<PaymentDto?> GetPaymentByReservationIdAsync(
            Guid reservationId, string userId, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);

            var payment = await _context.Payments
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ReservationId == reservationId && x.UserId == userId, cancellationToken);

            return payment == null ? null : MapToDto(payment);
        }

        public async Task<PagedResult<PaymentDto>> GetPaymentsAsync(
            PaymentSearchRequest request, CancellationToken cancellationToken = default)
        {
            var pageNumber = Math.Max(1, request.PageNumber);

            var pageSize = Math.Min(Math.Max(1, request.PageSize), MaxPageSize);

            var query = _context.Payments
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();

                query = query.Where(x =>
                    x.ReservationNumber.Contains(search) ||
                    x.PaymentReference!.Contains(search) ||
                    x.ProviderReference!.Contains(search)
                );
            }

            if (request.ReservationId.HasValue)
            {
                query = query.Where(x => x.ReservationId == request.ReservationId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.UserId))
            {
                query = query.Where(x => x.UserId == request.UserId);
            }

            if (request.VehicleId.HasValue)
            {
                query = query.Where(x => x.VehicleId == request.VehicleId.Value);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(x => x.Status == request.Status.Value);
            }

            if (request.Method.HasValue)
            {
                query = query.Where(x => x.Method == request.Method.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Provider))
            {
                query = query.Where(x => x.Provider == request.Provider);
            }

            if (request.FromDateUtc.HasValue)
            {
                query = query.Where(x => x.CreatedAtUtc >= request.FromDateUtc.Value);
            }

            if (request.ToDateUtc.HasValue)
            {
                query = query.Where(x => x.CreatedAtUtc <= request.ToDateUtc.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.CreatedAtUtc)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new PaymentDto(
                    x.Id,
                    x.ReservationId,
                    x.ReservationNumber,
                    x.UserId,
                    x.VehicleId,
                    x.Amount,
                    x.CurrencyCode,
                    x.Status,
                    x.Method,
                    x.Provider,
                    x.ProviderReference,
                    x.PaymentReference,
                    x.AuthorizationUrl,
                    x.CreatedAtUtc,
                    x.CompletedAtUtc))
                .ToListAsync(cancellationToken);

            return new PagedResult<PaymentDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PaymentStatisticsDto> GetStatisticsAsync(CancellationToken cancellationToken = default)
        {
            var query = _context.Payments.AsNoTracking();

            return new PaymentStatisticsDto
            {
                TotalPayments = await query.CountAsync(cancellationToken),

                PendingPayments = await query.CountAsync(x => 
                    x.Status == PaymentStatus.Pending, cancellationToken),

                ProcessingPayments = await query.CountAsync(x => 
                    x.Status == PaymentStatus.Processing, cancellationToken),

                AuthorizedPayments = await query.CountAsync(x => 
                    x.Status == PaymentStatus.Authorized, cancellationToken),

                CompletedPayments = await query.CountAsync(x => 
                    x.Status == PaymentStatus.Completed, cancellationToken),

                FailedPayments = await query.CountAsync(x => 
                    x.Status == PaymentStatus.Failed, cancellationToken),

                RefundedPayments = await query.CountAsync(x => 
                    x.Status == PaymentStatus.Refunded, cancellationToken),

                PartiallyRefundedPayments = await query.CountAsync(x => 
                    x.Status == PaymentStatus.PartiallyRefunded, cancellationToken),

                TotalAmount = await query.SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0,

                CompletedAmount = await query
                    .Where(x =>
                        x.Status == PaymentStatus.Completed ||
                        x.Status == PaymentStatus.PartiallyRefunded ||
                        x.Status == PaymentStatus.Refunded)
                    .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0,

                RefundedAmount = await query.SumAsync(x => 
                    (decimal?)x.RefundedAmount, cancellationToken) ?? 0,

                LastPaymentAtUtc = await query
                    .OrderByDescending(x => x.CreatedAtUtc)
                    .Select(x => (DateTime?)x.CreatedAtUtc)
                    .FirstOrDefaultAsync(cancellationToken)
            };
        }

        public async Task<PaymentDto> RefundPaymentAsync(Guid paymentId, 
            RefundPaymentRequest request, CancellationToken cancellationToken = default)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(x => x.Id == paymentId, cancellationToken)
                    ?? throw new KeyNotFoundException($"Payment '{paymentId}' was not found.");

            if (payment.Status != PaymentStatus.Completed &&
                payment.Status != PaymentStatus.PartiallyRefunded)
            {
                throw new InvalidOperationException(
                    "Only completed payments can be refunded.");
            }

            var refundAmount =
                request.Amount ??
                payment.Amount - payment.RefundedAmount;

            if (refundAmount <= 0)
            {
                throw new InvalidOperationException(
                    "Refund amount must be greater than zero.");
            }

            var remainingAmount = payment.Amount - payment.RefundedAmount;

            if (refundAmount > remainingAmount)
            {
                throw new InvalidOperationException(
                    "Refund amount cannot exceed the remaining payment amount.");
            }

            if (string.IsNullOrWhiteSpace(payment.ProviderReference))
            {
                throw new InvalidOperationException(
                    "Payment does not have a provider reference.");
            }

            payment.Status = PaymentStatus.RefundPending;
            payment.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            var provider = _providerResolver.Resolve(payment.Method);

            var result = await provider.RefundPaymentAsync(
                payment.ProviderReference,
                refundAmount,
                cancellationToken
            );

            if (!result.Success)
            {
                payment.Status = payment.RefundedAmount > 0
                    ? PaymentStatus.PartiallyRefunded
                    : PaymentStatus.Completed;
                payment.FailureReason = result.FailureReason;

                payment.UpdatedAtUtc = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                throw new InvalidOperationException(
                    result.FailureReason ??
                    "Payment refund failed."
                );
            }

            payment.RefundedAmount += refundAmount;
            payment.Status =
                payment.RefundedAmount >= payment.Amount
                    ? PaymentStatus.Refunded
                    : PaymentStatus.PartiallyRefunded;
            payment.RefundedAtUtc = DateTime.UtcNow;
            payment.UpdatedAtUtc = DateTime.UtcNow;

            AddTransaction(
                payment,
                refundAmount >= payment.Amount
                    ? PaymentTransactionType.Refund
                    : PaymentTransactionType.PartialRefund,
                payment.Status,
                refundAmount,
                payment.ProviderReference,
                request.Reason
            );

            await _context.SaveChangesAsync(cancellationToken);

            await _publishEndpoint.Publish(
                new PaymentRefundedIntegrationEvent(
                    payment.Id,
                    payment.ReservationId,
                    payment.ReservationNumber,
                    payment.UserId,
                    payment.VehicleId,
                    refundAmount,
                    payment.Amount,
                    payment.CurrencyCode,
                    payment.PaymentReference!,
                    payment.ProviderReference,
                    payment.RefundedAtUtc.Value),
                cancellationToken
            );

            return MapToDto(payment);
        }

        //public async Task<PaymentDto> VerifyPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default)
        //{
        //    var payment = await _context.Payments
        //        .FirstOrDefaultAsync(x => x.Id == paymentId, cancellationToken)
        //            ?? throw new KeyNotFoundException($"Payment '{paymentId}' was not found.");

        //    if (string.IsNullOrWhiteSpace(payment.ProviderReference))
        //    {
        //        throw new InvalidOperationException(
        //            "Payment does not have a provider reference.");
        //    }

        //    var provider = _providerResolver.Resolve(payment.Method);

        //    var result = await provider.VerifyPaymentAsync(
        //        payment.ProviderReference, cancellationToken
        //    );

        //    if (!result.Success)
        //    {
        //        payment.Status = PaymentStatus.Failed;
        //        payment.FailureReason = result.FailureReason;
        //        payment.FailedAtUtc = DateTime.UtcNow;
        //        payment.UpdatedAtUtc = DateTime.UtcNow;

        //        AddTransaction(
        //            payment,
        //            PaymentTransactionType.Payment,
        //            PaymentStatus.Failed,
        //            payment.Amount,
        //            payment.ProviderReference,
        //            result.FailureReason
        //        );

        //        await _context.SaveChangesAsync(cancellationToken);

        //        await _publishEndpoint.Publish(
        //            new PaymentFailedIntegrationEvent(
        //                payment.Id,
        //                payment.ReservationId,
        //                payment.ReservationNumber,
        //                payment.UserId,
        //                payment.VehicleId,
        //                payment.Amount,
        //                payment.CurrencyCode,
        //                payment.PaymentReference,
        //                result.FailureReason ?? "Payment verification failed.",
        //                payment.FailedAtUtc.Value),
        //            cancellationToken
        //        );

        //        return MapToDto(payment);
        //    }

        //    payment.Status = PaymentStatus.Completed;
        //    payment.CompletedAtUtc = DateTime.UtcNow;
        //    payment.UpdatedAtUtc = DateTime.UtcNow;

        //    AddTransaction(
        //        payment,
        //        PaymentTransactionType.Capture,
        //        PaymentStatus.Completed,
        //        payment.Amount,
        //        payment.ProviderReference,
        //        null
        //    );

        //    await _context.SaveChangesAsync(cancellationToken);

        //    await _publishEndpoint.Publish(
        //        new PaymentCompletedIntegrationEvent(
        //            payment.Id,
        //            payment.ReservationId,
        //            payment.ReservationNumber,
        //            payment.UserId,
        //            payment.VehicleId,
        //            payment.Amount,
        //            payment.CurrencyCode,
        //            payment.PaymentReference!,
        //            payment.ProviderReference,
        //            payment.CompletedAtUtc.Value),
        //        cancellationToken
        //    );

        //    return MapToDto(payment);
        //}

        public async Task<PaymentDto> VerifyPaymentAsync(Guid paymentId, string userId, CancellationToken cancellationToken = default)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(x => x.Id == paymentId && x.UserId == userId, cancellationToken)
                ?? throw new KeyNotFoundException($"Payment '{paymentId}' was not found.");

            // Idempotent verification.
            if (payment.Status == PaymentStatus.Completed ||
                payment.Status == PaymentStatus.Refunded ||
                payment.Status == PaymentStatus.PartiallyRefunded)
            {
                return MapToDto(payment);
            }

            //if (payment.Status == PaymentStatus.Completed)
            //{
            //    return MapToDto(payment);
            //}

            if (string.IsNullOrWhiteSpace(payment.ProviderReference))
            {
                throw new InvalidOperationException(
                    "Payment does not have a provider reference.");
            }

            var provider = _providerResolver.Resolve(payment.Method);

            var result = await provider.VerifyPaymentAsync(payment.ProviderReference, cancellationToken);

            if (!result.Success)
            {
                if (payment.Status == PaymentStatus.Completed)
                {
                    return MapToDto(payment);
                }

                payment.Status = PaymentStatus.Failed;
                payment.FailureReason = result.FailureReason ?? "Payment verification failed.";
                payment.FailedAtUtc = DateTime.UtcNow;
                payment.UpdatedAtUtc = DateTime.UtcNow;

                AddTransaction(
                    payment,
                    PaymentTransactionType.Payment,
                    PaymentStatus.Failed,
                    payment.Amount,
                    payment.ProviderReference,
                    payment.FailureReason
                );

                await _context.SaveChangesAsync(cancellationToken);

                await _publishEndpoint.Publish(
                    new PaymentFailedIntegrationEvent(
                        payment.Id,
                        payment.ReservationId,
                        payment.ReservationNumber,
                        payment.UserId,
                        payment.VehicleId,
                        payment.Amount,
                        payment.CurrencyCode,
                        payment.PaymentReference,
                        payment.FailureReason,
                        payment.FailedAtUtc.Value),
                    cancellationToken
                );

                return MapToDto(payment);
            }

            // Prevent a second completion caused by a concurrent request.
            if (payment.Status == PaymentStatus.Completed)
            {
                return MapToDto(payment);
            }

            payment.Status = PaymentStatus.Completed;
            payment.CompletedAtUtc = DateTime.UtcNow;
            payment.UpdatedAtUtc = DateTime.UtcNow;

            AddTransaction(
                payment,
                PaymentTransactionType.Capture,
                PaymentStatus.Completed,
                payment.Amount,
                payment.ProviderReference,
                null
            );

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                _context.Entry(payment).State = EntityState.Detached;

                var currentPayment = await _context.Payments
                    .AsNoTracking()
                    .FirstAsync(x => x.Id == paymentId, cancellationToken);

                return MapToDto(currentPayment);
            }

            await _publishEndpoint.Publish(
                new PaymentCompletedIntegrationEvent(
                    payment.Id,
                    payment.ReservationId,
                    payment.ReservationNumber,
                    payment.UserId,
                    payment.VehicleId,
                    payment.Amount,
                    payment.CurrencyCode,
                    payment.PaymentReference!,
                    payment.ProviderReference,
                    payment.CompletedAtUtc.Value
                ),
                cancellationToken
            );

            return MapToDto(payment);
        }

        private static void AddTransaction(Models.Payment payment,
            PaymentTransactionType type, PaymentStatus status, decimal amount,
            string? providerReference, string? response)
        {
            payment.Transactions.Add(
                new PaymentTransaction
                {
                    Id = Guid.NewGuid(),
                    PaymentId = payment.Id,
                    Type = type,
                    Status = status,
                    Amount = amount,
                    CurrencyCode = payment.CurrencyCode,
                    ProviderReference = providerReference,
                    ProviderResponse = response,
                    CreatedAtUtc = DateTime.UtcNow
                }
            );
        }

        private static PaymentDto MapToDto(Models.Payment payment)
        {
            return new PaymentDto(
                payment.Id,
                payment.ReservationId,
                payment.ReservationNumber,
                payment.UserId,
                payment.VehicleId,
                payment.Amount,
                payment.CurrencyCode,
                payment.Status,
                payment.Method,
                payment.Provider,
                payment.ProviderReference,
                payment.PaymentReference,
                payment.AuthorizationUrl,
                payment.CreatedAtUtc,
                payment.CompletedAtUtc
            );
        }

        public async Task ProcessPaystackWebhookAsync(string payload, string signature, CancellationToken cancellationToken = default)
        {
            var webhook = JsonSerializer
                .Deserialize<PaystackWebhookRequest>(payload,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (webhook == null)
            {
                throw new InvalidOperationException(
                    "Invalid Paystack webhook payload.");
            }

            if (string.IsNullOrWhiteSpace(webhook.Event))
            {
                return;
            }

            var reference = webhook.Data?.Reference;

            if (string.IsNullOrWhiteSpace(reference))
            {
                _logger.LogWarning(
                    "Paystack webhook {Event} contains no reference.",
                    webhook.Event
                );

                return;
            }

            var eventKey = $"{webhook.Event}:{reference}";

            var alreadyProcessed = await _context.PaymentWebhookEvents
                .AnyAsync(x => 
                    x.EventKey == eventKey && x.Processed, cancellationToken
                );

            if (alreadyProcessed)
            {
                _logger.LogInformation(
                    "Paystack webhook {EventKey} already processed.",
                    eventKey
                );

                return;
            }

            var webhookEvent = await _context.PaymentWebhookEvents
                .FirstOrDefaultAsync(x => x.EventKey == eventKey, cancellationToken);

            if (webhookEvent == null)
            {
                webhookEvent = new PaymentWebhookEvent
                {
                    Id = Guid.NewGuid(),
                    EventType = webhook.Event,
                    EventKey = eventKey,
                    ProviderReference = reference,
                    Payload = payload,
                    ReceivedAtUtc = DateTime.UtcNow,
                    Processed = false
                };

                _context.PaymentWebhookEvents.Add(webhookEvent);

                try
                {
                    await _publishEndpoint.Publish(
                        new PaystackWebhookReceived(payload, signature), cancellationToken);

                    await _context.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateException)
                {
                    // Another webhook request inserted the same event.
                    return;
                }
            }

            switch (webhook.Event.ToLowerInvariant())
            {
                case "charge.success":
                    await ProcessChargeSuccessWebhookAsync(webhook, cancellationToken);
                    break;

                default:
                    _logger.LogInformation(
                        "Ignoring unsupported Paystack webhook event {Event}.",
                        webhook.Event);
                    break;
            }

            webhookEvent.Processed = true;
            webhookEvent.ProcessedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task ProcessChargeSuccessWebhookAsync(
            PaystackWebhookRequest webhook, CancellationToken cancellationToken)
        {
            var data = webhook.Data;

            if (data == null || string.IsNullOrWhiteSpace(data.Reference))
            {
                return;
            }

            var payment = await _context.Payments
                .FirstOrDefaultAsync(
                    x => x.ProviderReference == data.Reference ||
                        x.PaymentReference == data.Reference,
                    cancellationToken
                );

            if (payment == null)
            {
                _logger.LogWarning(
                    "No ParkLink payment found for Paystack reference {Reference}.",
                    data.Reference
                );

                return;
            }

            // Already completed means webhook is a duplicate.
            if (payment.Status == PaymentStatus.Completed ||
                payment.Status == PaymentStatus.Refunded ||
                payment.Status == PaymentStatus.PartiallyRefunded)
            {
                return;
            }

            var webhookAmount = data.Amount / 100m;
            if (webhookAmount != payment.Amount)
            {
                throw new InvalidOperationException(
                    $"Paystack amount mismatch for payment {payment.Id}. " +
                    $"Expected {payment.Amount}, received {webhookAmount}.");
            }

            if (!string.Equals(data.Currency, payment.CurrencyCode, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Paystack currency mismatch for payment {payment.Id}.");
            }

            // Verify with Paystack before giving the transaction a completed state.
            var provider = _providerResolver.Resolve(payment.Method);
            var verification = await provider.VerifyPaymentAsync(data.Reference, cancellationToken);

            if (!verification.Success)
            {
                payment.Status = PaymentStatus.Failed;

                payment.FailureReason =
                    verification.FailureReason ??
                    "Paystack webhook verification failed.";

                payment.FailedAtUtc = DateTime.UtcNow;
                payment.UpdatedAtUtc = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                return;
            }

            payment.Status = PaymentStatus.Completed;
            payment.CompletedAtUtc = DateTime.UtcNow;
            payment.UpdatedAtUtc = DateTime.UtcNow;

            AddTransaction(
                payment,
                PaymentTransactionType.Capture,
                PaymentStatus.Completed,
                payment.Amount,
                payment.ProviderReference,
                "Completed via Paystack webhook."
            );

            await _context.SaveChangesAsync(cancellationToken);

            await _publishEndpoint.Publish(
                new PaymentCompletedIntegrationEvent(
                    payment.Id,
                    payment.ReservationId,
                    payment.ReservationNumber,
                    payment.UserId,
                    payment.VehicleId,
                    payment.Amount,
                    payment.CurrencyCode,
                    payment.PaymentReference!,
                    payment.ProviderReference,
                    payment.CompletedAtUtc.Value),
                cancellationToken
            );
        }
    }
}
