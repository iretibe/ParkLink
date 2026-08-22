namespace ParkLink.SharedKernel.Events.Gate
{
    public sealed record GateAccessRequestedIntegrationEvent(
        Guid AccessAttemptId,
        Guid GateId,
        Guid? VehicleId,
        Guid? ReservationId,
        string? LicensePlate,
        DateTime RequestedAtUtc
    );
}
