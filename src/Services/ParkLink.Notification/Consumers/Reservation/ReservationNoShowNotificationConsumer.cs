using MassTransit;
using ParkLink.Notification.Enums;
using ParkLink.Notification.Services;
using ParkLink.SharedKernel.Events.Reservation;

namespace ParkLink.Notification.Consumers.Reservation
{
    public sealed class ReservationNoShowNotificationConsumer
        : NotificationConsumerBase<ReservationNoShowIntegrationEvent>
    {
        public ReservationNoShowNotificationConsumer(
            INotificationService notificationService,
            INotificationDispatcher dispatcher,
            ILogger<ReservationNoShowNotificationConsumer> logger)
            : base(notificationService, dispatcher, logger)
        {
        }

        public override async Task Consume(
            ConsumeContext<ReservationNoShowIntegrationEvent> context)
        {
            LogReceived(context);

            var message = context.Message;

            await CreateNotificationAsync(
                context,
                message.UserId,
                nameof(ReservationNoShowIntegrationEvent),
                "Reservation marked as no-show",
                $"Your parking reservation {message.ReservationNumber} has been marked as a no-show.",
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
