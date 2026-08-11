namespace ParkLink.SharedKernel.Events.Reservation
{
    public sealed record ReservationConfirmedIntegrationEvent(
        Guid ReservationId,
        string ReservationNumber,
        string UserId,
        Guid VehicleId,
        Guid ParkingLotId,
        Guid ParkingZoneId,
        Guid ParkingSlotId,
        DateTime StartTimeUtc,
        DateTime EndTimeUtc,
        decimal Amount,
        string CurrencyCode,
        string? PaymentReference,
        string? AccessCredential,
        string AccessMethod
    ) : IntegrationEvent;
}
