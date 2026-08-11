using MassTransit;
using ParkLink.Notification.Enums;
using ParkLink.Notification.Services;
using ParkLink.SharedKernel.Events.Vehicle;

namespace ParkLink.Notification.Consumers.Vehicle
{
    public sealed class VehicleDeletedNotificationConsumer
        : NotificationConsumerBase<VehicleDeletedIntegrationEvent>
    {
        public VehicleDeletedNotificationConsumer(
            INotificationService notificationService,
            INotificationDispatcher dispatcher,
            ILogger<VehicleDeletedNotificationConsumer> logger)
            : base(notificationService, dispatcher, logger)
        {
        }

        public override async Task Consume(ConsumeContext<VehicleDeletedIntegrationEvent> context)
        {
            LogReceived(context);

            var message = context.Message;

            await CreateNotificationAsync(
                context,
                message.OwnerId,
                nameof(VehicleDeletedIntegrationEvent),
                "Vehicle deleted",
                $"Your vehicle {message.LicensePlateNumber} has been successfully removed from ParkLink.",
                entityId: message.VehicleId,
                entityType: "Vehicle",
                NotificationType.InApp,
                NotificationPriority.Normal,
                $"/vehicles/{message.VehicleId}",
                context.CorrelationId?.ToString(),
                context.CancellationToken
            );
        }
    }
}
