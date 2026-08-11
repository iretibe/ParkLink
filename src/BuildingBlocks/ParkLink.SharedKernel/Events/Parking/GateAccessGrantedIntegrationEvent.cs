namespace ParkLink.SharedKernel.Events.Parking
{
    public sealed record GateAccessGrantedIntegrationEvent(
        Guid RequestId,
        Guid ParkingLotId,
        Guid ParkingGateId,
        string VehicleId,
        string AccessDirection,
        string Reason,
        DateTime GrantedAtUtc
    ) : IntegrationEvent;
}
