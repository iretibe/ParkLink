namespace ParkLink.SharedKernel.Events.Payment
{
    public sealed record PaymentAuthorizedIntegrationEvent(
        Guid PaymentId,
        Guid ReservationId,
        string PaymentReference,
        decimal Amount,
        string CurrencyCode,
        DateTime AuthorizedAtUtc
    ) : IntegrationEvent;
}
