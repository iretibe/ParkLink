namespace ParkLink.SharedKernel.Events.Payment
{
    public sealed record PaymentRefundedIntegrationEvent(
        Guid PaymentId,
        Guid ReservationId,
        string ReservationNumber,
        string UserId,
        Guid VehicleId,
        decimal RefundedAmount,
        decimal OriginalAmount,
        string CurrencyCode,
        string PaymentReference,
        string? ProviderReference,
        DateTime RefundedAtUtc
    ) : IntegrationEvent;
}
