namespace ParkLink.SharedKernel.Events.Gate
{
    public sealed record GateAccessDeniedIntegrationEvent(
        Guid AccessAttemptId,
        Guid GateId,
        Guid? VehicleId,
        Guid? ReservationId,
        string? LicensePlate,
        string Reason,
        DateTime DeniedAtUtc
    );
}
