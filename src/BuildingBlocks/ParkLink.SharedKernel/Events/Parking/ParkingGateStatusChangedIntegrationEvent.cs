namespace ParkLink.SharedKernel.Events.Parking
{
    public sealed record ParkingGateStatusChangedIntegrationEvent(
        Guid ParkingGateId, Guid ParkingLotId, string PreviousStatus, 
        string NewStatus, DateTime ChangedAtUtc        
    ) : IntegrationEvent;
}
