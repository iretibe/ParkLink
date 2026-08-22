namespace ParkLink.SharedKernel.Events.Gate
{
    public sealed record VehicleDetectedAtGateIntegrationEvent(
        Guid AccessAttemptId,
        Guid GateId,
        Guid? VehicleId,
        string? LicensePlate,
        DateTime DetectedAtUtc
    );
}
