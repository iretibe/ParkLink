namespace ParkLink.SharedKernel.Events.Reservation
{
    public sealed record ReservationHoldCreatedIntegrationEvent(
        Guid HoldId,
        Guid ReservationId,
        string UserId,
        Guid VehicleId,
        Guid ParkingLotId,
        Guid ParkingZoneId,
        Guid ParkingSlotId,
        string ParkingSlotName,
        string ReservationNumber,
        DateTime CreatedAtUtc,
        DateTime ExpiresAtUtc
    ) : IntegrationEvent;
}
