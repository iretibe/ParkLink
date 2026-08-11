namespace ParkLink.SharedKernel.Events.Parking
{
    public sealed record ParkingSlotStatusChangedIntegrationEvent(
        Guid ParkingSlotId, Guid ParkingZoneId, Guid ParkingLotId,
        string SlotNumber, string PreviousStatus, string NewStatus, 
        DateTime StatusChangedAtUtc
    ) : IntegrationEvent;
}
