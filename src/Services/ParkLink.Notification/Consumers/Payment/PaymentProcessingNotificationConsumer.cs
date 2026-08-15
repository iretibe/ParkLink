using MassTransit;
using ParkLink.Notification.Enums;
using ParkLink.Notification.Services;
using ParkLink.SharedKernel.Events.Payment;

namespace ParkLink.Notification.Consumers.Payment
{
    public sealed class PaymentProcessingNotificationConsumer 
        : NotificationConsumerBase<PaymentProcessingIntegrationEvent>
    {
        public PaymentProcessingNotificationConsumer(
            INotificationService notificationService,
            INotificationDispatcher dispatcher,
            ILogger<PaymentProcessingNotificationConsumer> logger)
            : base(notificationService, dispatcher, logger)
        {
        }

        public override async Task Consume(ConsumeContext<PaymentProcessingIntegrationEvent> context)
        {
            LogReceived(context);

            var message = context.Message;

            await CreateNotificationAsync(
                context,
                message.UserId,
                nameof(PaymentProcessingIntegrationEvent),
                "Payment processing",
                $"Payment for reservation {message.ReservationNumber} is being processed.",
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
