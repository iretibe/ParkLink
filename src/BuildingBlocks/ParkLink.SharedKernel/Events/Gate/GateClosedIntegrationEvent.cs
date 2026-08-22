namespace ParkLink.SharedKernel.Events.Gate
{
    public sealed record GateClosedIntegrationEvent(
        Guid GateId,
        Guid DeviceId,
        Guid? AccessAttemptId,
        DateTime ClosedAtUtc
    );
}
