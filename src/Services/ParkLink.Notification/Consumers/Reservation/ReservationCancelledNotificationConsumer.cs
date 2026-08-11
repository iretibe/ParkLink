using MassTransit;
using ParkLink.Notification.Enums;
using ParkLink.Notification.Services;
using ParkLink.SharedKernel.Events.Reservation;

namespace ParkLink.Notification.Consumers.Reservation
{
    public sealed class ReservationCancelledNotificationConsumer
        : NotificationConsumerBase<ReservationCancelledIntegrationEvent>
    {
        public ReservationCancelledNotificationConsumer(
            INotificationService notificationService,
            INotificationDispatcher dispatcher,
            ILogger<ReservationCancelledNotificationConsumer> logger)
            : base(notificationService, dispatcher, logger)
        {
        }

        public override async Task Consume(ConsumeContext<ReservationCancelledIntegrationEvent> context)
        {
            LogReceived(context);

            var message = context.Message;

            await CreateNotificationAsync(
                context,
                message.UserId,
                nameof(ReservationCancelledIntegrationEvent),
                "Reservation cancelled",
                $"Your parking reservation {message.ReservationNumber} has been cancelled.",
                message.ReservationId,
                "Reservation",
                NotificationType.InApp | NotificationType.Email | NotificationType.Push,
                NotificationPriority.Normal,
                $"/reservations/{message.ReservationId}",
                context.CorrelationId?.ToString(),
                cancellationToken: context.CancellationToken
            );
        }
    }
}
