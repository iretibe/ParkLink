namespace ParkLink.SharedKernel.Events.Gate
{
    public sealed record GateAccessCompletedIntegrationEvent(
        Guid AccessAttemptId,
        Guid GateId,
        Guid VehicleId,
        Guid? ReservationId,
        string LicensePlate,
        DateTime CompletedAtUtc
    );
}
