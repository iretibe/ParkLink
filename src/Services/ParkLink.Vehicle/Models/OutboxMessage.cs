namespace ParkLink.Vehicle.Models
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
        public DateTime OccurredAtUtc { get; set; }        
        public DateTime? ProcessedAtUtc { get; set; }
        public int RetryCount { get; set; }
        public string? Error { get; set; }
        public DateTime? NextAttemptAtUtc { get; set; }
        public bool IsDeadLettered { get; set; }
        public DateTime? DeadLetteredAtUtc { get; set; }
        public string? LastAttemptedAtUtc { get; set; }
        public string? CorrelationId { get; set; }
    }
}
