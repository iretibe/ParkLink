using MassTransit;
using Microsoft.EntityFrameworkCore;
using ParkLink.Reservation.Data;
using ParkLink.Reservation.Enums;
using ParkLink.Shared.Contracts.Enums;
using ParkLink.SharedKernel.Events.Payment;
using ParkLink.SharedKernel.Events.Reservation;

namespace ParkLink.Reservation.Messaging.Consumers
{
    public sealed class PaymentCompletedConsumer
        : IConsumer<PaymentCompletedIntegrationEvent>
    {
        private readonly ReservationContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<PaymentCompletedConsumer> _logger;

        public PaymentCompletedConsumer(ReservationContext context,
            IPublishEndpoint publishEndpoint,
            ILogger<PaymentCompletedConsumer> logger)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Consume(
            ConsumeContext<PaymentCompletedIntegrationEvent> context)
        {
            var message = context.Message;

            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(
                    x => x.Id == message.ReservationId,
                    context.CancellationToken);

            if (reservation == null)
            {
                _logger.LogWarning(
                    "Reservation {ReservationId} was not found.",
                    message.ReservationId);

                return;
            }

            if (reservation.PaymentStatus == ReservationPaymentStatus.Paid)
            {
                return;
            }

            reservation.PaymentStatus = ReservationPaymentStatus.Paid;
            reservation.PaymentReference = message.PaymentReference;
            reservation.Status = ReservationStatus.Confirmed;
            reservation.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(context.CancellationToken);

            await _publishEndpoint.Publish(
                new ReservationConfirmedIntegrationEvent(
                    reservation.Id,
                    reservation.ReservationNumber,
                    reservation.UserId,
                    reservation.VehicleId,
                    reservation.ParkingLotId,
                    reservation.ParkingZoneId,
                    reservation.ParkingSlotId,
                    reservation.StartTimeUtc,
                    reservation.EndTimeUtc,
                    reservation.Amount,
                    reservation.CurrencyCode,
                    reservation.PaymentReference,
                    reservation.AccessCredential,
                    reservation.AccessMethod.ToString()),
                context.CancellationToken);

            _logger.LogInformation(
                "Reservation {ReservationId} confirmed after successful payment.",
                reservation.Id);
        }
    }
}
