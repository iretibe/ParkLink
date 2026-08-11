namespace ParkLink.Notification.Dtos
{
    public sealed class NotificationSearchRequest
    {
        public string? Search { get; set; }
        public string? Type { get; set; }
        public string? Priority { get; set; }
        public string? Channel { get; set; }
        public bool? IsRead { get; set; }
        public string? EntityType { get; set; }
        public DateTime? FromDateUtc { get; set; }
        public DateTime? ToDateUtc { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
