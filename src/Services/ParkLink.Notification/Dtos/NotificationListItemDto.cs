namespace ParkLink.Notification.Dtos
{
    public sealed class NotificationListItemDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public Guid? EntityId { get; set; }
        public string? EntityType { get; set; }
        public string? ActionUrl { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? ReadAtUtc { get; set; }
    }
}
