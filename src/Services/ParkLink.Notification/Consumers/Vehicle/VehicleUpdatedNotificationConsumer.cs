using MassTransit;
using ParkLink.Notification.Enums;
using ParkLink.Notification.Services;
using ParkLink.SharedKernel.Events.Vehicle;

namespace ParkLink.Notification.Consumers.Vehicle
{
    public sealed class VehicleUpdatedNotificationConsumer
        : NotificationConsumerBase<VehicleUpdatedIntegrationEvent>
    {
        public VehicleUpdatedNotificationConsumer(
            INotificationService notificationService,
            INotificationDispatcher dispatcher,
            ILogger<VehicleUpdatedNotificationConsumer> logger)
            : base(notificationService, dispatcher, logger)
        {
        }

        public override async Task Consume(ConsumeContext<VehicleUpdatedIntegrationEvent> context)
        {
            LogReceived(context);

            var message = context.Message;

            await CreateNotificationAsync(
                context,
                message.OwnerId,
                nameof(VehicleUpdatedIntegrationEvent),
                "Vehicle updated",
                $"Your vehicle {message.LicensePlateNumber} information has been updated.",
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
