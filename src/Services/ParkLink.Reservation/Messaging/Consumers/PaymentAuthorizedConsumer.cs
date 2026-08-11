using MassTransit;
using Microsoft.EntityFrameworkCore;
using ParkLink.Reservation.Data;
using ParkLink.Reservation.Enums;
using ParkLink.SharedKernel.Events.Payment;

namespace ParkLink.Reservation.Messaging.Consumers
{
    public sealed class PaymentAuthorizedConsumer
        : IConsumer<PaymentAuthorizedIntegrationEvent>
    {
        private readonly ReservationContext _context;
        private readonly ILogger<PaymentAuthorizedConsumer> _logger;

        public PaymentAuthorizedConsumer(ReservationContext context, 
            ILogger<PaymentAuthorizedConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(
            ConsumeContext<PaymentAuthorizedIntegrationEvent> context)
        {
            var message = context.Message;

            using var scope = _logger.BeginScope(
                new Dictionary<string, object?>
                {
                    ["EventId"] = message.EventId,
                    ["CorrelationId"] = context.CorrelationId?.ToString()
                });

            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(x => 
                    x.Id == message.ReservationId, context.CancellationToken);

            if (reservation == null)
            {
                _logger.LogWarning(
                    "Reservation {ReservationId} not found for payment authorization.",
                    message.ReservationId);

                return;
            }

            if (reservation.Status == ReservationStatus.Confirmed)
            {
                return;
            }

            if (reservation.Status is ReservationStatus.Cancelled or
                ReservationStatus.Completed or ReservationStatus.Expired)
            {
                _logger.LogWarning(
                    "Ignoring payment authorization for reservation {ReservationId} " +
                    "because it is already {Status}.",
                    reservation.Id,
                    reservation.Status);

                return;
            }

            reservation.PaymentStatus = ReservationPaymentStatus.Authorized;
            reservation.PaymentReference = message.PaymentReference;
            reservation.Status = ReservationStatus.Confirmed;
            reservation.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(context.CancellationToken);

            _logger.LogInformation(
                "Reservation {ReservationId} confirmed after payment authorization.",
                reservation.Id
            );
        }
    }
}
