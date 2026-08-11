using MassTransit;
using ParkLink.Notification.Enums;
using ParkLink.Notification.Services;
using ParkLink.SharedKernel.Events.Reservation;

namespace ParkLink.Notification.Consumers.Reservation
{
    public sealed class ReservationExpiredNotificationConsumer
        : NotificationConsumerBase<ReservationExpiredIntegrationEvent>
    {
        public ReservationExpiredNotificationConsumer(
            INotificationService notificationService,
            INotificationDispatcher dispatcher,
            ILogger<ReservationExpiredNotificationConsumer> logger)
            : base(notificationService, dispatcher, logger)
        {
        }

        public override async Task Consume(ConsumeContext<ReservationExpiredIntegrationEvent> context)
        {
            LogReceived(context);

            var message = context.Message;

            await CreateNotificationAsync(
                context,
                message.UserId,
                nameof(ReservationExpiredIntegrationEvent),
                "Reservation expired",
                $"Your parking reservation {message.ReservationNumber} has expired.",
                message.ReservationId,
                "Reservation",
                NotificationType.InApp | NotificationType.Email,
                NotificationPriority.Normal,
                $"/reservations/{message.ReservationId}",
                context.CorrelationId?.ToString(),
                cancellationToken: context.CancellationToken
            );
        }
    }
}
