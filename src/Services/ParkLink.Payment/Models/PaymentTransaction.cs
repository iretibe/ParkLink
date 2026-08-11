using ParkLink.Payment.Enums;
using System.ComponentModel.DataAnnotations;

namespace ParkLink.Payment.Models
{
    public class PaymentTransaction
    {
        public Guid Id { get; set; }
        public Guid PaymentId { get; set; }
        public PaymentTransactionType Type { get; set; }
        public PaymentStatus Status { get; set; }
        public decimal Amount { get; set; }
        [Required]
        [MaxLength(3)]
        public string CurrencyCode { get; set; } = "GHS";
        [MaxLength(200)]
        public string? ProviderReference { get; set; }
        [MaxLength(500)]
        public string? ProviderResponse { get; set; }
        [MaxLength(500)]
        public string? FailureReason { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public Payment Payment { get; set; } = default!;
    }
}
