using MassTransit;
using ParkLink.Notification.Enums;
using ParkLink.Notification.Services;
using ParkLink.SharedKernel.Events;

namespace ParkLink.Notification.Consumers
{
    public abstract class NotificationConsumerBase<TMessage>
        : IConsumer<TMessage> where TMessage : class
    {
        protected readonly INotificationService _notificationService;
        protected readonly INotificationDispatcher _dispatcher;
        protected readonly ILogger _logger;

        protected NotificationConsumerBase(INotificationService notificationService,
            INotificationDispatcher dispatcher, ILogger logger)
        {
            _notificationService = notificationService;
            _dispatcher = dispatcher;
            _logger = logger;
        }

        public abstract Task Consume(ConsumeContext<TMessage> context);

        protected async Task CreateNotificationAsync(
            ConsumeContext<TMessage> context, string userId, 
            string eventType, string title, string message,
            Guid? entityId = null, string? entityType = null,
            NotificationType channels = NotificationType.InApp | NotificationType.Email,
            NotificationPriority priority = NotificationPriority.Normal, 
            string? actionUrl = null, string? correlationId = null,
            CancellationToken cancellationToken = default)
        {
            var eventId = GetEventId(context);

            var notification = await _notificationService.CreateAsync(
                userId, eventId, eventType, title, message, 
                channels, priority, entityId, typeof(TMessage).FullName, 
                actionUrl, correlationId ?? context.CorrelationId?.ToString(),
                context.CancellationToken
            );

            await _dispatcher.DispatchAsync(notification, context.CancellationToken);
        }

        protected static Guid GetEventId(ConsumeContext<TMessage> context)
        {
            if (context.Message is IntegrationEvent integrationEvent)
            {
                return integrationEvent.EventId;
            }

            return context.MessageId ?? Guid.NewGuid();
        }

        protected void LogReceived(ConsumeContext<TMessage> context)
        {
            _logger.LogInformation(
                "Received {EventType}. MessageId={MessageId}, CorrelationId={CorrelationId}",
                typeof(TMessage).Name,
                context.MessageId,
                context.CorrelationId
            );
        }
    }
}
