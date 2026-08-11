namespace ParkLink.SharedKernel.Events.Reservation
{
    public sealed record ReservationCreatedIntegrationEvent(
        Guid ReservationId,
        string ReservationNumber,
        string UserId,
        Guid VehicleId,
        Guid ParkingLotId,
        Guid ParkingZoneId,
        Guid ParkingSlotId,
        string ParkingLotName,
        DateTime StartTimeUtc,
        DateTime EndTimeUtc,
        decimal Amount,
        string CurrencyCode
    ) : IntegrationEvent;
}
