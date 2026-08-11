namespace ParkLink.SharedKernel.Events.Reservation
{
    public sealed record ReservationUpdatedIntegrationEvent(
        Guid ReservationId,
        string ReservationNumber,
        Guid ParkingLotId,
        Guid ParkingZoneId,
        Guid ParkingSlotId,
        DateTime StartTimeUtc,
        DateTime EndTimeUtc,
        decimal Amount,
        string CurrencyCode
    ) : IntegrationEvent;
}
