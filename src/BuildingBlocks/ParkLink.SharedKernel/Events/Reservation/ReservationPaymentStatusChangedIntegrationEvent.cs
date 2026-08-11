namespace ParkLink.SharedKernel.Events.Reservation
{
    public sealed record ReservationPaymentStatusChangedIntegrationEvent(
        Guid ReservationId,
        string UserId,
        string? PaymentStatus
    ) : IntegrationEvent;
}
