using MassTransit;
using ParkLink.Notification.Enums;
using ParkLink.Notification.Services;
using ParkLink.SharedKernel.Events.Payment;

namespace ParkLink.Notification.Consumers.Payment
{
    public sealed class PaymentRefundedNotificationConsumer
        : NotificationConsumerBase<PaymentRefundedIntegrationEvent>
    {
        public PaymentRefundedNotificationConsumer(
            INotificationService notificationService,
            INotificationDispatcher dispatcher,
            ILogger<PaymentRefundedNotificationConsumer> logger)
            : base(notificationService, dispatcher, logger)
        {
        }

        public override async Task Consume(ConsumeContext<PaymentRefundedIntegrationEvent> context)
        {
            LogReceived(context);

            var message = context.Message;

            var title = message.RefundedAmount >= message.OriginalAmount
                ? "Payment refunded"
                : "Payment partially refunded";

            var text = message.RefundedAmount >= message.OriginalAmount
                ? $"Your payment of {message.OriginalAmount:N2} {message.CurrencyCode} for reservation {message.ReservationNumber} has been refunded."
                : $"A refund of {message.RefundedAmount:N2} {message.CurrencyCode} has been issued for reservation {message.ReservationNumber}.";

            await CreateNotificationAsync(
                context,
                message.UserId,
                nameof(PaymentRefundedIntegrationEvent),
                title,
                text,
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
