namespace ParkLink.SharedKernel.Events.Gate
{
    public sealed record LicensePlateRecognizedIntegrationEvent(
        Guid AccessAttemptId,
        Guid GateId,
        Guid DeviceId,
        string LicensePlate,
        double Confidence,
        DateTime RecognizedAtUtc
    );
}
