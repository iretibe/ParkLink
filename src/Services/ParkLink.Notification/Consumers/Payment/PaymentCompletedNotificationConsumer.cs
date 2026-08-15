using MassTransit;
using ParkLink.Notification.Enums;
using ParkLink.Notification.Services;
using ParkLink.SharedKernel.Events.Payment;

namespace ParkLink.Notification.Consumers.Payment
{
    public sealed class PaymentCompletedNotificationConsumer
        : NotificationConsumerBase<PaymentCompletedIntegrationEvent>
    {
        public PaymentCompletedNotificationConsumer(
            INotificationService notificationService,
            INotificationDispatcher dispatcher,
            ILogger<PaymentCompletedNotificationConsumer> logger)
            : base(notificationService, dispatcher, logger)
        {
        }

        public override async Task Consume(ConsumeContext<PaymentCompletedIntegrationEvent> context)
        {
            LogReceived(context);

            var message = context.Message;

            await CreateNotificationAsync(
                context,
                message.UserId,
                nameof(PaymentCompletedIntegrationEvent),
                "Payment completed",
                $"Your payment of {message.Amount:N2} {message.CurrencyCode} for reservation {message.ReservationNumber} has been completed successfully.",
                message.PaymentId,
                "Payment",
                NotificationType.InApp |
                NotificationType.Push |
                NotificationType.Email,
                NotificationPriority.High,
                $"/payments/{message.PaymentId}",
                context.CorrelationId?.ToString(),
                cancellationToken: context.CancellationToken
            );
        }
    }
}
