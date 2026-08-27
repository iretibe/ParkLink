using MassTransit;
using ParkLink.SharedKernel.Events.Vehicle;

namespace ParkLink.Gate.Messaging.Consumers.Vehicle
{
    public sealed class VehicleUpdatedConsumer
        : IConsumer<VehicleUpdatedIntegrationEvent>
    {
        private readonly ILogger<VehicleUpdatedConsumer> _logger;

        public VehicleUpdatedConsumer(ILogger<VehicleUpdatedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<VehicleUpdatedIntegrationEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation(
                "Vehicle updated. " +
                "VehicleId: {VehicleId}, " +
                "OwnerId: {OwnerId}, " +
                "LicensePlateNumber: {LicensePlateNumber}, " +
                "Make: {Make}, " +
                "Model: {Model}",
                message.VehicleId,
                message.OwnerId,
                message.LicensePlateNumber,
                message.Make,
                message.Model
            );

            // Update/invalidate local Gate vehicle cache.

            return Task.CompletedTask;
        }
    }
}
