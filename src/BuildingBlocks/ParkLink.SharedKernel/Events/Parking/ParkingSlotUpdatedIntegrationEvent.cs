namespace ParkLink.SharedKernel.Events.Parking
{
    public record ParkingSlotUpdatedIntegrationEvent(
        Guid ParkingSlotId,
        Guid ParkingZoneId,
        string SlotNumber,
        string SlotType,
        string Status,
        bool IsActive,
        DateTime UpdatedAtUtc
    ) : IntegrationEvent;
}
