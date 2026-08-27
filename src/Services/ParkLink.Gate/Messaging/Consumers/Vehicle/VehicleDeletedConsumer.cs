using MassTransit;
using ParkLink.SharedKernel.Events.Vehicle;

namespace ParkLink.Gate.Messaging.Consumers.Vehicle
{
    public sealed class VehicleDeletedConsumer
        : IConsumer<VehicleDeletedIntegrationEvent>
    {
        private readonly ILogger<VehicleDeletedConsumer> _logger;

        public VehicleDeletedConsumer(ILogger<VehicleDeletedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<VehicleDeletedIntegrationEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation(
                "Vehicle deleted. " +
                "VehicleId: {VehicleId}, " +
                "OwnerId: {OwnerId}, " +
                "LicensePlateNumber: {LicensePlateNumber}",
                message.VehicleId,
                message.OwnerId,
                message.LicensePlateNumber
            );

            // Remove/invalidate local Gate projection.

            return Task.CompletedTask;
        }
    }
}
