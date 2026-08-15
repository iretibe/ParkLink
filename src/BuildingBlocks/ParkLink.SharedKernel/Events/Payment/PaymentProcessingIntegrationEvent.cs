namespace ParkLink.SharedKernel.Events.Payment
{
    public sealed record PaymentProcessingIntegrationEvent(
        Guid PaymentId,
        Guid ReservationId,
        string ReservationNumber,
        string UserId,
        Guid VehicleId,
        decimal Amount,
        string CurrencyCode,
        string? PaymentReference,
        DateTime ProcessingStartedAtUtc
    ) : IntegrationEvent;
}
