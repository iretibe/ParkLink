using MassTransit;
using ParkLink.SharedKernel.Events.Vehicle;

namespace ParkLink.Gate.Messaging.Consumers.Vehicle
{
    public sealed class VehicleCreatedConsumer
        : IConsumer<VehicleCreatedIntegrationEvent>
    {
        private readonly ILogger<VehicleCreatedConsumer> _logger;

        public VehicleCreatedConsumer(ILogger<VehicleCreatedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<VehicleCreatedIntegrationEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation(
                "Vehicle created. " +
                "VehicleId: {VehicleId}, " +
                "OwnerId: {OwnerId}, " +
                "LicensePlateNumber: {LicensePlateNumber}, " +
                "Make: {Make}, " +
                "Model: {Model}, " +
                "VehicleType: {VehicleType}",
                message.VehicleId,
                message.OwnerId,
                message.LicensePlateNumber,
                message.Make,
                message.Model,
                message.VehicleType
            );

            // Update/invalidate Gate's local vehicle cache if required.

            return Task.CompletedTask;
        }
    }
}
