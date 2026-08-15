using MassTransit;
using ParkLink.Notification.Enums;
using ParkLink.Notification.Services;
using ParkLink.SharedKernel.Events.Payment;

namespace ParkLink.Notification.Consumers.Payment
{
    public sealed class PaymentAuthorizedNotificationConsumer
        : NotificationConsumerBase<PaymentAuthorizedIntegrationEvent>
    {
        public PaymentAuthorizedNotificationConsumer(
            INotificationService notificationService,
            INotificationDispatcher dispatcher,
            ILogger<PaymentAuthorizedNotificationConsumer> logger)
            : base(notificationService, dispatcher, logger)
        {
        }

        public override async Task Consume(
            ConsumeContext<PaymentAuthorizedIntegrationEvent> context)
        {
            LogReceived(context);

            var message = context.Message;

            await CreateNotificationAsync(
                context,
                message.UserId,
                nameof(PaymentAuthorizedIntegrationEvent),
                "Payment authorized",
                $"Payment of {message.Amount:N2} {message.CurrencyCode} for reservation {message.ReservationNumber} has been authorized.",
                message.PaymentId,
                "Payment",
                NotificationType.InApp | NotificationType.Push,
                NotificationPriority.Normal,
                $"/payments/{message.PaymentId}",
                context.CorrelationId?.ToString(),
                cancellationToken: context.CancellationToken
            );
        }
    }
}
