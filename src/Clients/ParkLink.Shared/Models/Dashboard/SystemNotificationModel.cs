namespace ParkLink.Shared.Models.Dashboard
{
    public sealed class SystemNotificationModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = "Info";
        public DateTime CreatedAtUtc { get; set; }
        public string TimeAgo { get; set; } = string.Empty;
        public bool IsRead { get; set; }
    }
}
