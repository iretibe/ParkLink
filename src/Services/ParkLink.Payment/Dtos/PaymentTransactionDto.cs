using ParkLink.Payment.Enums;

namespace ParkLink.Payment.Dtos
{
    public sealed class PaymentTransactionDto
    {
        public Guid Id { get; init; }
        public Guid PaymentId { get; init; }
        public PaymentTransactionType Type { get; init; }
        public PaymentStatus Status { get; init; }
        public decimal Amount { get; init; }
        public string CurrencyCode { get; init; } = string.Empty;
        public string? ProviderReference { get; init; }
        public string? ProviderResponse { get; init; }
        public string? FailureReason { get; init; }
        public DateTime CreatedAtUtc { get; init; }
    }
}
