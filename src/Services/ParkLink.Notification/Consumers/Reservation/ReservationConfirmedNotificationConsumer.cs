using MassTransit;
using ParkLink.Notification.Enums;
using ParkLink.Notification.Services;
using ParkLink.SharedKernel.Events.Reservation;

namespace ParkLink.Notification.Consumers.Reservation
{
    public sealed class ReservationConfirmedNotificationConsumer
        : NotificationConsumerBase<ReservationConfirmedIntegrationEvent>
    {
        public ReservationConfirmedNotificationConsumer(
            INotificationService notificationService,
            INotificationDispatcher dispatcher,
            ILogger<ReservationConfirmedNotificationConsumer> logger)
            : base(notificationService, dispatcher, logger)
        {
        }

        public override async Task Consume(ConsumeContext<ReservationConfirmedIntegrationEvent> context)
        {
            LogReceived(context);

            var message = context.Message;

            await CreateNotificationAsync(
                context,
                message.UserId,
                nameof(ReservationConfirmedIntegrationEvent),
                "Reservation confirmed",
                $"Your parking reservation {message.ReservationNumber} has been confirmed.",
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
