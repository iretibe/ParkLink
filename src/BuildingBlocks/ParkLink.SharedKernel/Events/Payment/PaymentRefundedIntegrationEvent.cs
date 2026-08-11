namespace ParkLink.SharedKernel.Events.Payment
{
    public sealed record PaymentRefundedIntegrationEvent(
        Guid PaymentId,
        Guid ReservationId,
        string PaymentReference,
        decimal Amount,
        string CurrencyCode,
        DateTime RefundedAtUtc
    ) : IntegrationEvent;
}
