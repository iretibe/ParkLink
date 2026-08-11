using ParkLink.Notification.Enums;
using System.ComponentModel.DataAnnotations;

namespace ParkLink.Notification.Models
{
    public sealed class Notification
    {
        public Guid Id { get; set; }

        // The integration event that caused this notification.
        // Used as part of the idempotency boundary.
        public Guid EventId { get; set; }
        
        // Name of the integration event.
       // Example: ReservationConfirmedIntegrationEvent.
        [Required]
        [MaxLength(200)]
        public string EventType { get; set; } = string.Empty;

        // Target Identity user.
        [Required]
        [MaxLength(450)]
        public string UserId { get; set; } = string.Empty;

        // Optional entity associated with this notification.
        // Example: ReservationId, VehicleId, ParkingLotId.
        public Guid? EntityId { get; set; }

        // Notification title.
        [Required]
        [MaxLength(250)]
        public string Title { get; set; } = string.Empty;

        // Notification body.
        [Required]
        [MaxLength(4000)]
        public string Message { get; set; } = string.Empty;

        // Optional deep link within the application.
        // Example: /reservations/{id}
        [MaxLength(1000)]
        public string? ActionUrl { get; set; }

        // Type of the entity associated with this notification.
        [MaxLength(100)]
        public string? EntityType { get; set; }

        public NotificationType Type { get; set; } = NotificationType.InApp; 

        public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

        public NotificationStatus Status { get; set; } = NotificationStatus.Pending;

        // Whether the user has read the in-app notification.
        public bool IsRead { get; set; }

        public DateTime? ReadAtUtc { get; set; }

        // Number of delivery attempts.
        public int DeliveryAttempts { get; set; }

        public DateTime? LastAttemptAtUtc { get; set; }

        public DateTime? DeliveredAtUtc { get; set; }

        public DateTime? FailedAtUtc { get; set; }

        [MaxLength(2000)]
        public string? Error { get; set; }

        public int RetryCount { get; set; }

        // Optional expiration time.
        public DateTime? ExpiresAtUtc { get; set; }

        // Correlation ID propagated from the originating request/event.
        [MaxLength(100)]
        public string? CorrelationId { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAtUtc { get; set; }
    }
}
