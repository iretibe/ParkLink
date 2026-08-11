namespace ParkLink.SharedKernel.Events.Reservation
{
    public sealed record ReservationActivatedIntegrationEvent(
        Guid ReservationId,
        string ReservationNumber,
        string UserId,
        Guid VehicleId,
        Guid ParkingLotId,
        Guid ParkingZoneId,
        Guid ParkingSlotId,
        DateTime? ActualEntryTimeUtc
    ) : IntegrationEvent;
}
