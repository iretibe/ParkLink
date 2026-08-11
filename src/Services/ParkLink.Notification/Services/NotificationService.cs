using Microsoft.EntityFrameworkCore;
using ParkLink.Notification.Data;
using ParkLink.Notification.Enums;
using ParkLink.SharedKernel.Events.Reservation;

namespace ParkLink.Notification.Services
{
    public class NotificationService : INotificationService
    {
        private readonly NotificationContext _context;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(NotificationContext context,
            ILogger<NotificationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Models.Notification> CreateAsync(string userId, 
            Guid eventId, string eventType, string title, string message, 
            NotificationType type = NotificationType.InApp, 
            NotificationPriority priority = NotificationPriority.Normal,
            Guid? entityId = null, string? entityType = null, 
            string? actionUrl = null, string? correlationId = null, 
            CancellationToken cancellationToken = default)
        {
            var existing = await _context.Notifications
                .FirstOrDefaultAsync(
                    x =>
                        x.EventId == eventId &&
                        x.UserId == userId,
                    cancellationToken
                );

            if (existing != null)
            {
                _logger.LogDebug(
                    "Notification already exists for EventId {EventId} and UserId {UserId}.",
                    eventId,
                    userId);

                return existing;
            }

            var notification = new Models.Notification
            {
                Id = Guid.NewGuid(),
                EventId = eventId,
                EventType = eventType,
                UserId = userId,
                EntityId = entityId,
                EntityType = entityType,
                Title = title,
                Message = message,
                ActionUrl = actionUrl,
                Type = type,
                Priority = priority,
                Status = NotificationStatus.Pending,
                IsRead = false,
                CorrelationId = correlationId,
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Notification {NotificationId} created for user {UserId}.",
                notification.Id,
                userId);

            return notification;
        }

        public Task<bool> ExistsAsync(Guid eventId, string userId, CancellationToken cancellationToken = default)
        {
            return _context.Notifications
                .AnyAsync(x =>
                    x.EventId == eventId && x.UserId == userId, cancellationToken);
        }

        public async Task MarkAllAsReadAsync(string userId, CancellationToken cancellationToken = default)
        {
            var notifications = await _context.Notifications
                .Where(x => x.UserId == userId && !x.IsRead)
                .ToListAsync(cancellationToken);

            if (notifications.Count == 0)
            {
                return;
            }

            var now = DateTime.UtcNow;

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadAtUtc = now;
                notification.UpdatedAtUtc = now;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task MarkAsReadAsync(Guid notificationId, string userId, CancellationToken cancellationToken = default)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(x =>
                    x.Id == notificationId && x.UserId == userId,
                    cancellationToken
                );

            if (notification == null)
            {
                throw new KeyNotFoundException(
                    $"Notification '{notificationId}' was not found.");
            }

            if (notification.IsRead)
            {
                return;
            }

            notification.IsRead = true;
            notification.ReadAtUtc = DateTime.UtcNow;
            notification.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }

        public Task SendReservationCancelledAsync(ReservationCancelledIntegrationEvent message, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task SendReservationCompletedAsync(ReservationCompletedIntegrationEvent message, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task SendReservationConfirmedAsync(ReservationConfirmedIntegrationEvent message, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task SendReservationExpiredAsync(ReservationExpiredIntegrationEvent message, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task SendVehicleNotificationAsync(Guid userId, string title, string message, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
