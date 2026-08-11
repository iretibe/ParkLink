using ParkLink.Payment.Enums;

namespace ParkLink.Payment.Dtos
{
    public sealed record CreatePaymentRequest(
        Guid ReservationId,
        string ReservationNumber,
        Guid VehicleId,
        decimal Amount,
        string CurrencyCode,
        string CustomerEmail,
        PaymentMethod Method,
        string? CallbackUrl
    );
}
