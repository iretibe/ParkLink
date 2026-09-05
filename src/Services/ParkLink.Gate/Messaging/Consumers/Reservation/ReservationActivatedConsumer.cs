using MassTransit;
using ParkLink.SharedKernel.Events.Reservation;

namespace ParkLink.Gate.Messaging.Consumers.Reservation
{
    public sealed class ReservationActivatedConsumer
        : IConsumer<ReservationActivatedIntegrationEvent>
    {
        private readonly ILogger<ReservationActivatedConsumer> _logger;

        public ReservationActivatedConsumer(ILogger<ReservationActivatedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<ReservationActivatedIntegrationEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation(
                "Reservation activated. " +
                "ReservationId: {ReservationId}, " +
                "ReservationNumber: {ReservationNumber}, " +
                "VehicleId: {VehicleId}, " +
                "ParkingSlotId: {ParkingSlotId}, " +
                "ActualEntryTimeUtc: {ActualEntryTimeUtc}",
                message.ReservationId,
                message.ReservationNumber,
                message.VehicleId,
                message.ParkingSlotId,
                message.ActualEntryTimeUtc
            );

            // Update local Gate access state if necessary.

            return Task.CompletedTask;
        }
    }
}
