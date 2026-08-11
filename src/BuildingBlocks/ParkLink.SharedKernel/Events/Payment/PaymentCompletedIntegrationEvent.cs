namespace ParkLink.SharedKernel.Events.Payment
{
    public sealed record PaymentCompletedIntegrationEvent(
        Guid PaymentId,
        Guid ReservationId,
        string ReservationNumber,
        string UserId,
        Guid VehicleId,
        decimal Amount,
        string CurrencyCode,
        string PaymentReference,
        string? ProviderReference,
        DateTime CompletedAtUtc
    ) : IntegrationEvent;
}
