using Microsoft.EntityFrameworkCore;
using ParkLink.Notification.Data;
using ParkLink.Notification.Dtos;
using ParkLink.Notification.Enums;
using ParkLink.SharedKernel.Pagination;

namespace ParkLink.Notification.Services
{
    public class NotificationService : INotificationService
    {
        private const int DefaultPageSize = 20;
        private const int MaxPageSize = 100;

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

        public async Task DeleteNotificationAsync(Guid notificationId, string userId, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == notificationId && x.UserId == userId,
                    cancellationToken
                );

            if (notification == null)
            {
                throw new KeyNotFoundException(
                    $"Notification '{notificationId}' was not found.");
            }

            _context.Notifications.Remove(notification);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Notification {NotificationId} deleted for user {UserId}.",
                notificationId,
                userId);
        }

        public Task<bool> ExistsAsync(Guid eventId, string userId, CancellationToken cancellationToken = default)
        {
            return _context.Notifications
                .AnyAsync(x =>
                    x.EventId == eventId && x.UserId == userId, cancellationToken);
        }

        public async Task<NotificationDetailsDto?> GetNotificationByIdAsync(Guid notificationId, string userId, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);

            var notification = await _context.Notifications
                .AsNoTracking()
                .FirstOrDefaultAsync(x => 
                    x.Id == notificationId && x.UserId == userId, cancellationToken
                );

            if (notification == null)
            {
                return null;
            }

            return MapToDetailsDto(notification);
        }

        public async Task<PagedResult<NotificationListItemDto>> GetNotificationsAsync(
            string userId, NotificationSearchRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);

            request ??= new NotificationSearchRequest();

            var query = BuildUserNotificationQuery(userId);

            query = ApplyFilters(query, request);

            return await ToPagedResultAsync(query, request, cancellationToken);
        }

        public async Task<NotificationStatisticsDto> GetStatisticsAsync(string userId, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);

            var query = _context.Notifications
                .AsNoTracking()
                .Where(x => x.UserId == userId);

            var statistics = new NotificationStatisticsDto
            {
                TotalNotifications = await query.CountAsync(cancellationToken),

                UnreadNotifications = await query
                    .CountAsync(x => !x.IsRead, cancellationToken),

                ReadNotifications = await query
                    .CountAsync(x => x.IsRead, cancellationToken),

                ExpiredNotifications = await query.CountAsync(x =>
                    x.ExpiresAtUtc.HasValue &&
                    x.ExpiresAtUtc.Value <= DateTime.UtcNow, cancellationToken),

                HighPriorityNotifications = await query.CountAsync(
                    x => x.Priority.ToString() == "High", cancellationToken),

                CriticalPriorityNotifications = await query.CountAsync(
                    x => x.Priority.ToString() == "Critical", cancellationToken),

                LastNotificationAtUtc = await query
                    .OrderByDescending(x => x.CreatedAtUtc)
                    .Select(x => (DateTime?)x.CreatedAtUtc)
                    .FirstOrDefaultAsync(cancellationToken)
            };

            return statistics;
        }

        public async Task<PagedResult<NotificationListItemDto>> GetUnreadNotificationsAsync(
            string userId, NotificationSearchRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);

            var query = BuildUserNotificationQuery(userId)
                .Where(x => !x.IsRead);

            query = ApplyFilters(query, request);

            return await ToPagedResultAsync(query, request, cancellationToken);
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

        private IQueryable<Models.Notification> BuildUserNotificationQuery(string userId)
        {
            return _context.Notifications
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAtUtc);
        }

        private static IQueryable<Models.Notification> ApplyFilters(
            IQueryable<Models.Notification> query, NotificationSearchRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();

                query = query.Where(x =>
                    x.Title.Contains(search) || x.Message.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.Type))
            {
                var type = request.Type.Trim();

                query = query.Where(x => x.Type.ToString() == type);
            }

            if (!string.IsNullOrWhiteSpace(request.Priority))
            {
                var priority = request.Priority.Trim();

                query = query.Where(x => x.Priority.ToString() == priority);
            }

            if (!string.IsNullOrWhiteSpace(request.Channel))
            {
                var channel = request.Channel.Trim();

                query = query.Where(x => x.Type.ToString() == channel);
            }

            if (request.IsRead.HasValue)
            {
                query = query.Where(x => x.IsRead == request.IsRead.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.EntityType))
            {
                var entityType = request.EntityType.Trim();

                query = query.Where(x => x.EntityType == entityType);
            }

            if (request.FromDateUtc.HasValue)
            {
                query = query.Where(x => x.CreatedAtUtc >= request.FromDateUtc.Value);
            }

            if (request.ToDateUtc.HasValue)
            {
                query = query.Where(x => x.CreatedAtUtc <= request.ToDateUtc.Value);
            }

            return query;
        }

        private static async Task<PagedResult<NotificationListItemDto>>
            ToPagedResultAsync(IQueryable<Models.Notification> query,
            NotificationSearchRequest request, CancellationToken cancellationToken)
        {
            var pageNumber = request.PageNumber < 1
                ? 1
                : request.PageNumber;

            var pageSize = request.PageSize < 1
                ? DefaultPageSize
                : Math.Min(
                    request.PageSize,
                    MaxPageSize);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new NotificationListItemDto
                {
                    Id = x.Id,
                    Type = x.Type.ToString(),
                    Title = x.Title,
                    Message = x.Message,
                    IsRead = x.IsRead,
                    Priority = x.Priority.ToString(),
                    Channel = x.Type.ToString(),
                    EntityId = x.EntityId,
                    EntityType = x.EntityType,
                    ActionUrl = x.ActionUrl,
                    CreatedAtUtc = x.CreatedAtUtc,
                    ReadAtUtc = x.ReadAtUtc
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<NotificationListItemDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        private static NotificationDetailsDto MapToDetailsDto(Models.Notification entity)
        {
            return new NotificationDetailsDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Type = entity.Type.ToString(),
                Title = entity.Title,
                Message = entity.Message,
                Data = entity.Message,
                IsRead = entity.IsRead,
                ReadAtUtc = entity.ReadAtUtc,
                Priority = entity.Priority.ToString(),
                Channel = entity.Type.ToString(),
                EntityId = entity.EntityId,
                EntityType = entity.EntityType,
                ActionUrl = entity.ActionUrl,
                CorrelationId = entity.CorrelationId,
                CreatedAtUtc = entity.CreatedAtUtc,
                ExpiresAtUtc = entity.ExpiresAtUtc
            };
        }
    }
}
