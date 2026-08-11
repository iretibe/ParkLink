using MassTransit;
using ParkLink.Notification.Enums;
using ParkLink.Notification.Services;
using ParkLink.SharedKernel.Events.Vehicle;

namespace ParkLink.Notification.Consumers.Vehicle
{
    public sealed class VehicleStatusChangedNotificationConsumer
        : NotificationConsumerBase<VehicleStatusChangedIntegrationEvent>
    {
        public VehicleStatusChangedNotificationConsumer(
            INotificationService notificationService,
            INotificationDispatcher dispatcher,
            ILogger<VehicleStatusChangedNotificationConsumer> logger)
            : base(notificationService, dispatcher, logger)
        {
        }

        public override async Task Consume(ConsumeContext<VehicleStatusChangedIntegrationEvent> context)
        {
            LogReceived(context);

            var message = context.Message;

            await CreateNotificationAsync(
                context,
                message.OwnerId,
                nameof(VehicleStatusChangedIntegrationEvent),
                "Vehicle status changed",
                $"Your vehicle {message.LicensePlateNumber} status has changed to {message.NewStatus}.",
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
