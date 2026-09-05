namespace ParkLink.SharedKernel.Events.Gate
{
    public sealed record GateAccessGrantedIntegrationEvent(
        Guid AccessAttemptId,
        Guid GateId,
        Guid VehicleId,
        Guid? ReservationId,
        string? LicensePlate,
        DateTime GrantedAtUtc
    );
}
