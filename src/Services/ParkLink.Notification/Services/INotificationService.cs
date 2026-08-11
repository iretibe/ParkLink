using ParkLink.Notification.Dtos;
using ParkLink.Notification.Enums;
using ParkLink.SharedKernel.Pagination;

namespace ParkLink.Notification.Services
{
    public interface INotificationService
    {
        Task<Models.Notification> CreateAsync(string userId, 
            Guid eventId, string eventType, string title, string message,
            NotificationType type = NotificationType.InApp,
            NotificationPriority priority = NotificationPriority.Normal,
            Guid? entityId = null, string? entityType = null,
            string? actionUrl = null, string? correlationId = null,
            CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid eventId, string userId,
            CancellationToken cancellationToken = default);
        Task MarkAsReadAsync(Guid notificationId, string userId,
            CancellationToken cancellationToken = default);
        Task MarkAllAsReadAsync(string userId,
            CancellationToken cancellationToken = default);
        Task<PagedResult<NotificationListItemDto>> GetNotificationsAsync(
            string userId, NotificationSearchRequest request,
            CancellationToken cancellationToken = default);
        Task<PagedResult<NotificationListItemDto>> GetUnreadNotificationsAsync(
            string userId, NotificationSearchRequest request,
            CancellationToken cancellationToken = default);
        Task<NotificationDetailsDto?> GetNotificationByIdAsync(
            Guid notificationId, string userId,
            CancellationToken cancellationToken = default);
        Task DeleteNotificationAsync(Guid notificationId, string userId,
            CancellationToken cancellationToken = default);
        Task<NotificationStatisticsDto> GetStatisticsAsync(string userId,
            CancellationToken cancellationToken = default);


        //Task SendReservationConfirmedAsync(
        //    ReservationConfirmedIntegrationEvent message,
        //    CancellationToken cancellationToken = default);
        //Task SendReservationCancelledAsync(
        //    ReservationCancelledIntegrationEvent message,
        //    CancellationToken cancellationToken = default);
        //Task SendReservationExpiredAsync(
        //    ReservationExpiredIntegrationEvent message,
        //    CancellationToken cancellationToken = default);
        //Task SendReservationCompletedAsync(
        //    ReservationCompletedIntegrationEvent message,
        //    CancellationToken cancellationToken = default);
        //Task SendVehicleNotificationAsync(Guid userId,
        //    string title, string message,
        //    CancellationToken cancellationToken = default);
    }
}
