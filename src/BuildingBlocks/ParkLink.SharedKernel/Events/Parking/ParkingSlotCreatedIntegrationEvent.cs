namespace ParkLink.SharedKernel.Events.Parking
{
    public sealed record ParkingSlotCreatedIntegrationEvent(
        Guid ParkingSlotId, Guid ParkingZoneId, Guid ParkingLotId, 
        string SlotNumber, string SlotType, string Status
    ) : IntegrationEvent;
}
