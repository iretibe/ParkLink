namespace ParkLink.SharedKernel.Events.Gate
{
    public sealed record GateDeviceOnlineIntegrationEvent(
        Guid GateId,
        Guid DeviceId,
        string DeviceType,
        DateTime DetectedAtUtc
    );
}
