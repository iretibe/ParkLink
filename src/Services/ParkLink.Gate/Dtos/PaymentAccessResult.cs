namespace ParkLink.Gate.Dtos
{
    public sealed record PaymentAccessResult(
        Guid PaymentId, Guid ReservationId,
        decimal Amount, string CurrencyCode,
        string Status, bool IsValid
    );
}
