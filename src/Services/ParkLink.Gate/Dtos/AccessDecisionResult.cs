using ParkLink.Gate.Enums;

namespace ParkLink.Gate.Dtos
{
    public sealed record AccessDecisionResult(
        Guid AccessAttemptId, AccessDecision Decision,
        string Reason, Guid? VehicleId, Guid? ReservationId,
        string? LicensePlate, string? RfidTagIdentifier,
        bool GateOpened
    );
}
