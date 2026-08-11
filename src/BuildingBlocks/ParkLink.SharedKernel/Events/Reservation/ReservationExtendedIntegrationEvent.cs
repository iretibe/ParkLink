namespace ParkLink.SharedKernel.Events.Reservation
{
    public sealed record ReservationExtendedIntegrationEvent(
        Guid ReservationId,
        string ReservationNumber,
        string UserId,
        Guid ParkingLotId,
        Guid ParkingZoneId,
        Guid ParkingSlotId,
        DateTime PreviousEndTimeUtc,
        DateTime NewEndTimeUtc,
        decimal AdditionalAmount,
        string CurrencyCode
    ) : IntegrationEvent;

    //public sealed record ReservationExtendedIntegrationEvent(
    //    Guid ReservationId,
    //    string ReservationNumber,
    //    string UserId,
    //    DateTime PreviousEndTimeUtc,
    //    DateTime NewEndTimeUtc,
    //    decimal Amount,
    //    string? Reason
    //) : IntegrationEvent;
}
