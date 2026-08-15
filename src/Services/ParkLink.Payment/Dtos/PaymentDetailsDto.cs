using ParkLink.Payment.Enums;

namespace ParkLink.Payment.Dtos
{
    public sealed class PaymentDetailsDto
    {
        public Guid Id { get; init; }
        public Guid ReservationId { get; init; }
        public string ReservationNumber { get; init; } = string.Empty;
        public string UserId { get; init; } = string.Empty;
        public Guid VehicleId { get; init; }
        public decimal Amount { get; init; }
        public string CurrencyCode { get; init; } = string.Empty;
        public PaymentStatus Status { get; init; }
        public PaymentMethod Method { get; init; }
        public string? Provider { get; init; }
        public string? ProviderReference { get; init; }
        public string? PaymentReference { get; init; }
        public string? FailureReason { get; init; }
        public DateTime? AuthorizedAtUtc { get; init; }
        public DateTime? CompletedAtUtc { get; init; }
        public DateTime? FailedAtUtc { get; init; }
        public DateTime? RefundedAtUtc { get; init; }
        public decimal RefundedAmount { get; init; }
        public DateTime CreatedAtUtc { get; init; }
        public DateTime? UpdatedAtUtc { get; init; }

        public IReadOnlyList<PaymentTransactionDto> Transactions { get; init; }
            = Array.Empty<PaymentTransactionDto>();
    }
}
