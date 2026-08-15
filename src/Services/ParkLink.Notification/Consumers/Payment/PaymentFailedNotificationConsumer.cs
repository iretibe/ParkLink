using MassTransit;
using ParkLink.Notification.Enums;
using ParkLink.Notification.Services;
using ParkLink.SharedKernel.Events.Payment;

namespace ParkLink.Notification.Consumers.Payment
{
    public sealed class PaymentFailedNotificationConsumer
        : NotificationConsumerBase<PaymentFailedIntegrationEvent>
    {
        public PaymentFailedNotificationConsumer(
            INotificationService notificationService,
            INotificationDispatcher dispatcher,
            ILogger<PaymentFailedNotificationConsumer> logger)
            : base(notificationService, dispatcher, logger)
        {
        }

        public override async Task Consume(ConsumeContext<PaymentFailedIntegrationEvent> context)
        {
            LogReceived(context);

            var message = context.Message;

            await CreateNotificationAsync(
                context,
                message.UserId,
                nameof(PaymentFailedIntegrationEvent),
                "Payment failed",
                $"Your payment for reservation {message.ReservationNumber} could not be completed. Reason: {message.FailureReason}",
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
