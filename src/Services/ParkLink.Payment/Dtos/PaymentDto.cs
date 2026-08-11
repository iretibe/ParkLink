using ParkLink.Payment.Enums;

namespace ParkLink.Payment.Dtos
{
    public sealed record PaymentDto(
        Guid Id,
        Guid ReservationId,
        string ReservationNumber,
        string UserId,
        Guid VehicleId,
        decimal Amount,
        string CurrencyCode,
        PaymentStatus Status,
        PaymentMethod Method,
        string? Provider,
        string? ProviderReference,
        string? PaymentReference,
        DateTime CreatedAtUtc,
        DateTime? CompletedAtUtc
    );
}
