namespace ParkLink.SharedKernel.Events.Gate
{
    public sealed record RfidTagDetectedIntegrationEvent(
        Guid AccessAttemptId,
        Guid GateId,
        Guid DeviceId,
        string TagUid,
        DateTime DetectedAtUtc
    );
}
