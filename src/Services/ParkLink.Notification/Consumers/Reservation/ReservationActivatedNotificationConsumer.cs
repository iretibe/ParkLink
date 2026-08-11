using MassTransit;
using ParkLink.Notification.Enums;
using ParkLink.Notification.Services;
using ParkLink.SharedKernel.Events.Reservation;

namespace ParkLink.Notification.Consumers.Reservation
{
    public sealed class ReservationActivatedNotificationConsumer
        : NotificationConsumerBase<ReservationActivatedIntegrationEvent>
    {
        public ReservationActivatedNotificationConsumer(
            INotificationService notificationService,
            INotificationDispatcher dispatcher,
            ILogger<ReservationActivatedNotificationConsumer> logger)
            : base(notificationService, dispatcher, logger)
        {
        }

        public override async Task Consume(ConsumeContext<ReservationActivatedIntegrationEvent> context)
        {
            LogReceived(context);

            var message = context.Message;

            await CreateNotificationAsync(
                context,
                message.UserId,
                nameof(ReservationActivatedIntegrationEvent),
                "Reservation activated",
                $"Your parking reservation {message.ReservationNumber} is now active.",
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
