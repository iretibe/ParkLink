namespace ParkLink.SharedKernel.Events.Gate
{
    public sealed record GateDeviceOfflineIntegrationEvent(
        Guid GateId,
        Guid DeviceId,
        string DeviceType,
        DateTime DetectedAtUtc,
        string? Reason
    );
}
