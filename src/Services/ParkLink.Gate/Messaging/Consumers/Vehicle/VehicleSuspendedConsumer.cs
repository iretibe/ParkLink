using MassTransit;
using ParkLink.SharedKernel.Events.Vehicle;

namespace ParkLink.Gate.Messaging.Consumers.Vehicle
{
    public sealed class VehicleSuspendedConsumer
        : IConsumer<VehicleSuspendedIntegrationEvent>
    {
        private readonly ILogger<VehicleSuspendedConsumer> _logger;

        public VehicleSuspendedConsumer(ILogger<VehicleSuspendedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<VehicleSuspendedIntegrationEvent> context)
        {
            var message = context.Message;

            _logger.LogWarning(
                "Vehicle suspended. " +
                "VehicleId: {VehicleId}, " +
                "OwnerId: {OwnerId}, " +
                "LicensePlateNumber: {LicensePlateNumber}, " +
                "SuspendedByUserId: {SuspendedByUserId}, " +
                "Reason: {Reason}",
                message.VehicleId,
                message.OwnerId,
                message.LicensePlateNumber,
                message.SuspendedByUserId,
                message.Reason
            );

            // Mark the vehicle as unavailable for Gate access
            // if Gate maintains a local projection/cache.

            return Task.CompletedTask;
        }
    }
}
