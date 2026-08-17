namespace ParkLink.Payment.Models
{
    public sealed class PaymentWebhookEvent
    {
        public Guid Id { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string EventKey { get; set; } = string.Empty;
        public string ProviderReference { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
        public bool Processed { get; set; }
        public DateTime ReceivedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAtUtc { get; set; }
        public string? FailureReason { get; set; }
    }
}
