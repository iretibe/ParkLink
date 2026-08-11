using MassTransit;
using ParkLink.Notification.Enums;
using ParkLink.Notification.Services;
using ParkLink.SharedKernel.Events.Reservation;

namespace ParkLink.Notification.Consumers.Reservation
{
    public sealed class ReservationCompletedNotificationConsumer
        : NotificationConsumerBase<ReservationCompletedIntegrationEvent>
    {
        public ReservationCompletedNotificationConsumer(
            INotificationService notificationService,
            INotificationDispatcher dispatcher,
            ILogger<ReservationCompletedNotificationConsumer> logger)
            : base(notificationService, dispatcher, logger)
        {
        }

        public override async Task Consume(
            ConsumeContext<ReservationCompletedIntegrationEvent> context)
        {
            LogReceived(context);

            var message = context.Message;

            await CreateNotificationAsync(
                context,
                message.UserId,
                nameof(ReservationCompletedIntegrationEvent),
                "Parking session completed",
                "Your parking session has been completed.",
                message.ReservationId,
                "Reservation",
                NotificationType.InApp | NotificationType.Push,
                NotificationPriority.Normal,
                $"/reservations/{message.ReservationId}",
                context.CorrelationId?.ToString(),
                cancellationToken: context.CancellationToken
            );
        }
    }
}
