using MassTransit;
using ParkLink.SharedKernel.Events.Reservation;

namespace ParkLink.Gate.Messaging.Consumers.Reservation
{
    public sealed class ReservationNoShowConsumer
        : IConsumer<ReservationNoShowIntegrationEvent>
    {
        private readonly ILogger<ReservationNoShowConsumer> _logger;

        public ReservationNoShowConsumer(ILogger<ReservationNoShowConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<ReservationNoShowIntegrationEvent> context)
        {
            var message = context.Message;

            _logger.LogWarning(
                "Reservation marked as no-show. " +
                "ReservationId: {ReservationId}, " +
                "ReservationNumber: {ReservationNumber}, " +
                "VehicleId: {VehicleId}, " +
                "ParkingSlotId: {ParkingSlotId}, " +
                "NoShowAtUtc: {NoShowAtUtc}",
                message.ReservationId,
                message.ReservationNumber,
                message.VehicleId,
                message.ParkingSlotId,
                message.NoShowAtUtc
            );

            // Invalidate Gate authorization for this reservation.

            return Task.CompletedTask;
        }
    }
}
