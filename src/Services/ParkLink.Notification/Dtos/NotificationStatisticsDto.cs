namespace ParkLink.Notification.Dtos
{
    public sealed class NotificationStatisticsDto
    {
        public int TotalNotifications { get; set; }
        public int UnreadNotifications { get; set; }
        public int ReadNotifications { get; set; }
        public int ExpiredNotifications { get; set; }
        public int HighPriorityNotifications { get; set; }
        public int CriticalPriorityNotifications { get; set; }
        public DateTime? LastNotificationAtUtc { get; set; }
    }
}
