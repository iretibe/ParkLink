namespace ParkLink.SharedKernel.Events
{
    public abstract record IntegrationEvent
    {
        public Guid EventId { get; init; } = Guid.NewGuid();
        public DateTime OccurredAtUtc { get; init; } = DateTime.UtcNow;
        public string? CorrelationId { get; init; } = Guid.NewGuid().ToString();
    }
}
