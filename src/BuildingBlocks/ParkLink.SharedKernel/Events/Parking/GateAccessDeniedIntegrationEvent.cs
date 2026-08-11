namespace ParkLink.SharedKernel.Events.Parking
{
    public sealed record GateAccessDeniedIntegrationEvent(
        Guid RequestId,
        Guid ParkingLotId,
        Guid ParkingGateId,
        string VehicleId,
        string AccessDirection,
        string Reason,
        DateTime DeniedAtUtc
    ) : IntegrationEvent;
}
