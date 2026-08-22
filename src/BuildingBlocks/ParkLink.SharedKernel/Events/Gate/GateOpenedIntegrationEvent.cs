namespace ParkLink.SharedKernel.Events.Gate
{
    public sealed record GateOpenedIntegrationEvent(
        Guid GateId,
        Guid DeviceId,
        Guid? AccessAttemptId,
        DateTime OpenedAtUtc
    );
}
