namespace ParkLink.SharedKernel.Events.Payment
{
    public sealed record PaymentFailedIntegrationEvent(
        Guid PaymentId,
        Guid ReservationId,
        string ReservationNumber,
        string UserId,
        Guid VehicleId,
        decimal Amount,
        string CurrencyCode,
        string? PaymentReference,
        string FailureReason,
        DateTime FailedAtUtc
    ) : IntegrationEvent;
}
