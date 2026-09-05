using MassTransit;
using ParkLink.SharedKernel.Events.Reservation;

namespace ParkLink.Gate.Messaging.Consumers.Reservation
{
    public sealed class ReservationCreatedConsumer
        : IConsumer<ReservationCreatedIntegrationEvent>
    {
        private readonly ILogger<ReservationCreatedConsumer> _logger;

        public ReservationCreatedConsumer(ILogger<ReservationCreatedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<ReservationCreatedIntegrationEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation(
                "Reservation created. " +
                "ReservationId: {ReservationId}, " +
                "ReservationNumber: {ReservationNumber}, " +
                "UserId: {UserId}, " +
                "VehicleId: {VehicleId}, " +
                "ParkingLotId: {ParkingLotId}, " +
                "ParkingZoneId: {ParkingZoneId}, " +
                "ParkingSlotId: {ParkingSlotId}, " +
                "StartTime: {StartTimeUtc}, " +
                "EndTime: {EndTimeUtc}",
                message.ReservationId,
                message.ReservationNumber,
                message.UserId,
                message.VehicleId,
                message.ParkingLotId,
                message.ParkingZoneId,
                message.ParkingSlotId,
                message.StartTimeUtc,
                message.EndTimeUtc
            );

            // Update Gate reservation projection/cache if required.

            return Task.CompletedTask;
        }
    }
}
