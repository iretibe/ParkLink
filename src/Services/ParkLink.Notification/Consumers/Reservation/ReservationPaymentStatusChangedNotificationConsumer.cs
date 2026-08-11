using MassTransit;
using ParkLink.Notification.Enums;
using ParkLink.Notification.Services;
using ParkLink.SharedKernel.Events.Reservation;

namespace ParkLink.Notification.Consumers.Reservation
{
    public sealed class ReservationPaymentStatusChangedNotificationConsumer
        : NotificationConsumerBase<ReservationPaymentStatusChangedIntegrationEvent>
    {
        public ReservationPaymentStatusChangedNotificationConsumer(
            INotificationService notificationService,
            INotificationDispatcher dispatcher,
            ILogger<ReservationPaymentStatusChangedNotificationConsumer> logger)
            : base(notificationService, dispatcher, logger)
        {
        }

        public override async Task Consume(
            ConsumeContext<ReservationPaymentStatusChangedIntegrationEvent> context)
        {
            LogReceived(context);

            var message = context.Message;

            var status =
                message.NewStatus?.ToString()
                ?? "Unknown";

            var channels =
                status.Equals(
                    "Failed",
                    StringComparison.OrdinalIgnoreCase)
                    ? NotificationType.InApp | NotificationType.Email | NotificationType.Push
                    : NotificationType.InApp | NotificationType.Email;

            await CreateNotificationAsync(
                context,
                message.UserId,
                nameof(ReservationPaymentStatusChangedIntegrationEvent),
                "Payment status updated",
                $"Your reservation payment status is now {status}.",
                message.ReservationId,
                "Reservation",
                channels,
                NotificationPriority.Normal,
                $"/reservations/{message.ReservationId}",
                context.CorrelationId?.ToString(),
                cancellationToken: context.CancellationToken
            );
        }
    }
}
