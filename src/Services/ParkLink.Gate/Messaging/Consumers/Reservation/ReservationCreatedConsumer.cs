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
               "VehicleId: {VehicleId}, " +
               "ParkingSlotId: {ParkingSlotId}",
               message.ReservationId,
               message.VehicleId,
               message.ParkingSlotId
            );

            // Update Gate reservation projection/cache if required.

            return Task.CompletedTask;
        }
    }
}
