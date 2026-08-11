using ParkLink.Notification.Enums;

namespace ParkLink.Notification.Services
{
    public class NotificationDispatcher : INotificationDispatcher
    {
        private readonly ILogger<NotificationDispatcher> _logger;

        public NotificationDispatcher(ILogger<NotificationDispatcher> logger)
        {
            _logger = logger;
        }

        public async Task DispatchAsync(Models.Notification notification, CancellationToken cancellationToken = default)
        {
            var channels = notification.Type;

            if (channels.HasFlag(NotificationType.InApp))
            {
                await SendInAppAsync(notification, cancellationToken);
            }

            if (channels.HasFlag(NotificationType.Email))
            {
                await SendEmailAsync(notification, cancellationToken);
            }

            if (channels.HasFlag(NotificationType.Push))
            {
                await SendPushAsync(notification, cancellationToken);
            }

            if (channels.HasFlag(NotificationType.Sms))
            {
                await SendSmsAsync(notification, cancellationToken);
            }
        }

        private Task SendInAppAsync(Models.Notification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "In-app notification created for user {UserId}. NotificationId={NotificationId}",
                notification.UserId,
                notification.Id);

            return Task.CompletedTask;
        }

        private Task SendEmailAsync(Models.Notification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Email notification requested for user {UserId}. NotificationId={NotificationId}",
                notification.UserId,
                notification.Id);

            // TODO:
            // Integrate SendGrid / SMTP / Azure Communication Services.

            return Task.CompletedTask;
        }

        private Task SendPushAsync(Models.Notification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Push notification requested for user {UserId}. NotificationId={NotificationId}",
                notification.UserId,
                notification.Id);

            // TODO:
            // Firebase Cloud Messaging / Azure Notification Hubs.

            return Task.CompletedTask;
        }

        private Task SendSmsAsync(Models.Notification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "SMS notification requested for user {UserId}. NotificationId={NotificationId}",
                notification.UserId,
                notification.Id);

            // TODO:
            // Twilio / Azure Communication Services / Africa's Talking / local SMS provider.

            return Task.CompletedTask;
        }
    }
}
