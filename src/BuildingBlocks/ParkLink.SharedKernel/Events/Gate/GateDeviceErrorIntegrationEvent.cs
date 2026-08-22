namespace ParkLink.SharedKernel.Events.Gate
{
    public sealed record GateDeviceErrorIntegrationEvent(
        Guid GateId,
        Guid DeviceId,
        string DeviceType,
        string ErrorCode,
        string ErrorMessage,
        DateTime OccurredAtUtc
    );
}
