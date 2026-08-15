using MassTransit;
using Microsoft.EntityFrameworkCore;
using ParkLink.Reservation.Data;
using ParkLink.Reservation.Enums;
using ParkLink.Shared.Contracts.Enums;
using ParkLink.SharedKernel.Events.Payment;

namespace ParkLink.Reservation.Messaging.Consumers
{
    public sealed class PaymentRefundedConsumer
        : IConsumer<PaymentRefundedIntegrationEvent>
    {
        private readonly ReservationContext _context;
        private readonly ILogger<PaymentRefundedConsumer> _logger;

        public PaymentRefundedConsumer(ReservationContext context,
            ILogger<PaymentRefundedConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(
            ConsumeContext<PaymentRefundedIntegrationEvent> context)
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
                    "Reservation {ReservationId} not found for refund.",
                    message.ReservationId);

                return;
            }

            reservation.PaymentStatus = ReservationPaymentStatus.Refunded;

            if (reservation.Status == ReservationStatus.Confirmed ||
                reservation.Status == ReservationStatus.Held)
            {
                reservation.Status = ReservationStatus.Cancelled;
                reservation.CancellationReason =
                    "Reservation cancelled following payment refund.";
                reservation.CancelledAtUtc = DateTime.UtcNow;
            }

            reservation.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(context.CancellationToken);

            _logger.LogInformation(
                "Reservation {ReservationId} processed after payment refund.",
                reservation.Id);
        }
    }
}
