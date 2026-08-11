namespace ParkLink.SharedKernel.Events.Reservation
{
    public sealed record ReservationCompletedIntegrationEvent(
        Guid ReservationId,
        string ReservationNumber,
        string UserId,
        Guid VehicleId,
        Guid ParkingLotId,
        Guid ParkingZoneId,
        Guid ParkingSlotId,
        DateTime ActualEntryTimeUtc,
        DateTime ActualExitTimeUtc,
        decimal Amount,
        string CurrencyCode
    ) : IntegrationEvent;
}
