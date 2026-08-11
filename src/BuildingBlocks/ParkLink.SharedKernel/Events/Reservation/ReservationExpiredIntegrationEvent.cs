namespace ParkLink.SharedKernel.Events.Reservation
{
    public sealed record ReservationExpiredIntegrationEvent(
        Guid ReservationId,
        string ReservationNumber,
        string UserId,
        Guid VehicleId,
        Guid ParkingLotId,
        Guid ParkingZoneId,
        Guid ParkingSlotId,
        DateTime ExpiredAtUtc
    ) : IntegrationEvent;
}
