namespace ParkLink.SharedKernel.Events.Parking
{
    public sealed record ParkingZoneCreatedIntegrationEvent(
        Guid ParkingZoneId, Guid ParkingLotId, string Name, 
        int Capacity, string Status
    ) : IntegrationEvent;
}
