namespace ParkLink.SharedKernel.Messaging
{
    public sealed class IntegrationEventEnvelope
    {
        public Guid EventId { get; init; }
        public string EventType { get; init; } = string.Empty;
        public DateTime OccurredAtUtc { get; init; }
        public string Payload { get; init; } = string.Empty;
        public string? CorrelationId { get; init; }
    }
}
