namespace ParkLink.SharedKernel.Events.Payment
{
    public sealed record PaymentCompletedIntegrationEvent(
        Guid PaymentId,
        Guid ReservationId,
        string PaymentReference,
        decimal Amount,
        string CurrencyCode,
        DateTime PaidAtUtc
    ) : IntegrationEvent;
}
