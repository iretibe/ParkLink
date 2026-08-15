namespace ParkLink.Payment.Dtos.Providers
{
    public sealed record PaymentProviderRequest(
        Guid PaymentId,
        Guid ReservationId,
        string ReservationNumber,
        string UserId,
        decimal Amount,
        string CurrencyCode,
        string CustomerEmail,
        string? CallbackUrl
    );
}
