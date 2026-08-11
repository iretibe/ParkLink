namespace ParkLink.SharedKernel.Events.Reservation
{
    public sealed record ReservationNoShowIntegrationEvent(
        Guid ReservationId,
        string ReservationNumber,
        string UserId,
        Guid VehicleId,
        Guid ParkingLotId,
        Guid ParkingZoneId,
        Guid ParkingSlotId,
        DateTime NoShowAtUtc
    ) : IntegrationEvent;
}
