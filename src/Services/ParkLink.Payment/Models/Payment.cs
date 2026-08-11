using ParkLink.Payment.Enums;
using System.ComponentModel.DataAnnotations;

namespace ParkLink.Payment.Models
{
    public class Payment
    {
        public Guid Id { get; set; }
        public Guid ReservationId { get; set; }
        [Required]
        [MaxLength(50)]
        public string ReservationNumber { get; set; } = string.Empty;
        [Required]
        [MaxLength(450)]
        public string UserId { get; set; } = string.Empty;
        public Guid VehicleId { get; set; }
        public decimal Amount { get; set; }
        [Required]
        [MaxLength(3)]
        public string CurrencyCode { get; set; } = "GHS";
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public PaymentMethod Method { get; set; }
        [MaxLength(100)]
        public string? Provider { get; set; }
        [MaxLength(200)]
        public string? ProviderReference { get; set; }
        [MaxLength(200)]
        public string? PaymentReference { get; set; }
        [MaxLength(500)]
        public string? FailureReason { get; set; }
        public DateTime? AuthorizedAtUtc { get; set; }
        public DateTime? CompletedAtUtc { get; set; }
        public DateTime? FailedAtUtc { get; set; }
        public DateTime? RefundedAtUtc { get; set; }
        public decimal RefundedAmount { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        public ICollection<PaymentTransaction> Transactions { get; set; } = new List<PaymentTransaction>();
    }
}
