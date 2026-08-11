using MassTransit;
using ParkLink.Notification.Enums;
using ParkLink.Notification.Services;
using ParkLink.SharedKernel.Events.Reservation;

namespace ParkLink.Notification.Consumers.Reservation
{
    public sealed class ReservationExtendedNotificationConsumer
        : NotificationConsumerBase<ReservationExtendedIntegrationEvent>
    {
        public ReservationExtendedNotificationConsumer(
            INotificationService notificationService,
            INotificationDispatcher dispatcher,
            ILogger<ReservationExtendedNotificationConsumer> logger)
            : base(notificationService, dispatcher, logger)
        {
        }

        public override async Task Consume(ConsumeContext<ReservationExtendedIntegrationEvent> context)
        {
            LogReceived(context);

            var message = context.Message;

            await CreateNotificationAsync(
                context,
                message.UserId,
                nameof(ReservationExtendedIntegrationEvent),
                "Reservation extended",
                $"Your parking reservation {message.ReservationNumber} has been successfully extended.",
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
