using MassTransit;
using ParkLink.Notification.Enums;
using ParkLink.Notification.Services;
using ParkLink.SharedKernel.Events.Vehicle;

namespace ParkLink.Notification.Consumers.Vehicle
{
    public sealed class VehicleCreatedNotificationConsumer
        : NotificationConsumerBase<VehicleCreatedIntegrationEvent>
    {
        public VehicleCreatedNotificationConsumer(
            INotificationService notificationService,
            INotificationDispatcher dispatcher,
            ILogger<VehicleCreatedNotificationConsumer> logger)
            : base(notificationService, dispatcher, logger)
        {
        }

        public override async Task Consume(
            ConsumeContext<VehicleCreatedIntegrationEvent> context)
        {
            LogReceived(context);

            var message = context.Message;

            await CreateNotificationAsync(
                context,
                message.OwnerId,
                nameof(VehicleCreatedIntegrationEvent),
                "Vehicle added",
                $"Your vehicle {message.LicensePlateNumber} has been successfully added to ParkLink.",
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
