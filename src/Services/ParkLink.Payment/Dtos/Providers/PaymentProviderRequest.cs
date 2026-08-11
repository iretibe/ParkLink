namespace ParkLink.Payment.Dtos.Providers
{
    public sealed record PaymentProviderRequest(
        Guid PaymentId,
        Guid ReservationId,
        decimal Amount,
        string CurrencyCode,
        string CustomerEmail,
        string? CallbackUrl
    );
}
