using MassTransit;
using ParkLink.Notification.Enums;
using ParkLink.Notification.Services;
using ParkLink.SharedKernel.Events.Reservation;

namespace ParkLink.Notification.Consumers.Reservation
{
    public sealed class ReservationHoldReleasedNotificationConsumer
        : NotificationConsumerBase<ReservationHoldReleasedIntegrationEvent>
    {
        public ReservationHoldReleasedNotificationConsumer(
            INotificationService notificationService,
            INotificationDispatcher dispatcher,
            ILogger<ReservationHoldReleasedNotificationConsumer> logger)
            : base(notificationService, dispatcher, logger)
        {
        }

        public override async Task Consume(ConsumeContext<ReservationHoldReleasedIntegrationEvent> context)
        {
            LogReceived(context);

            var message = context.Message;

            await CreateNotificationAsync(
                context,
                message.UserId,
                nameof(ReservationHoldReleasedIntegrationEvent),
                "Parking slot released",
                $"Your temporary parking slot hold '{message.ParkingSlotName}' has been released.",
                message.HoldId,
                "ReservationHold",
                NotificationType.InApp,
                NotificationPriority.Normal,
                $"/reservationHolds/{message.HoldId}",
                context.CorrelationId?.ToString(),
                cancellationToken: context.CancellationToken
            );
        }
    }
}
