namespace ParkLink.SharedKernel.Events.Reservation
{
    public sealed record ReservationPaymentStatusChangedIntegrationEvent(
        Guid ReservationId,
        string ReservationNumber,
        string UserId,
        Guid VehicleId,
        Guid ParkingLotId,
        Guid ParkingSlotId,
        string PreviousStatus,
        string NewStatus,
        decimal Amount,
        string CurrencyCode,
        string? PaymentReference,
        DateTime ChangedAtUtc
    ) : IntegrationEvent;
}
