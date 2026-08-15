using MassTransit;
using Microsoft.EntityFrameworkCore;
using ParkLink.Reservation.Data;
using ParkLink.Reservation.Enums;
using ParkLink.Shared.Contracts.Enums;
using ParkLink.SharedKernel.Events.Payment;

namespace ParkLink.Reservation.Messaging.Consumers
{
    public sealed class PaymentFailedConsumer
        : IConsumer<PaymentFailedIntegrationEvent>
    {
        private readonly ReservationContext _context;
        private readonly ILogger<PaymentFailedConsumer> _logger;

        public PaymentFailedConsumer(ReservationContext context, 
            ILogger<PaymentFailedConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(
            ConsumeContext<PaymentFailedIntegrationEvent> context)
        {
            var message = context.Message;

            using var scope = _logger.BeginScope(
                new Dictionary<string, object?>
                {
                    ["EventId"] = message.EventId,
                    ["CorrelationId"] = context.CorrelationId?.ToString()
                });

            var reservation = await _context.Reservations
                .Include(x => x.Hold)
                .FirstOrDefaultAsync(x => 
                    x.Id == message.ReservationId, context.CancellationToken);

            if (reservation == null)
            {
                _logger.LogWarning(
                    "Reservation {ReservationId} not found for payment failure.",
                    message.ReservationId);

                return;
            }

            if (reservation.PaymentStatus == ReservationPaymentStatus.Paid)
            {
                _logger.LogWarning(
                    "Ignoring payment failure for already-paid reservation {ReservationId}.",
                    reservation.Id);

                return;
            }

            reservation.PaymentStatus = ReservationPaymentStatus.Failed;

            if (reservation.Status == ReservationStatus.Held ||
                reservation.Status == ReservationStatus.Pending)
            {
                reservation.Status = ReservationStatus.Cancelled;
                reservation.CancellationReason = message.FailureReason;
                reservation.CancelledAtUtc = DateTime.UtcNow;
                reservation.UpdatedAtUtc = DateTime.UtcNow;
            }

            if (reservation.Hold != null &&
                reservation.Hold.Status == ReservationHoldStatus.Active)
            {
                reservation.Hold.Status = ReservationHoldStatus.Released;
                reservation.Hold.ReleasedAtUtc = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(context.CancellationToken);

            _logger.LogInformation(
                "Reservation {ReservationId} processed after payment failure.",
                reservation.Id);
        }
    }
}
