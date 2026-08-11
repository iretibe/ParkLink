using ParkLink.Reservation.Enums;
using System.ComponentModel.DataAnnotations;

namespace ParkLink.Reservation.Models
{
    public sealed class Reservation
    {
        public Guid Id { get; set; }

        // Identity Service
        [Required]
        [MaxLength(450)]
        public string UserId { get; set; } = string.Empty;

        // Business reference
        [Required]
        [MaxLength(50)]
        public string ReservationNumber { get; set; } = string.Empty;

        // Vehicle Service
        public Guid VehicleId { get; set; }

        // Parking Service
        public Guid ParkingLotId { get; set; }

        public Guid ParkingZoneId { get; set; }

        public Guid ParkingSlotId { get; set; }

        // Snapshot values
        [Required]
        [MaxLength(200)]
        public string ParkingLotName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string ParkingSlotNumber { get; set; } = string.Empty;

        public ReservationType ReservationType { get; set; }

        public ReservationStatus Status { get; set; }
            = ReservationStatus.Pending;

        public ReservationPaymentStatus PaymentStatus { get; set; }
            = ReservationPaymentStatus.Pending;

        public AccessMethod AccessMethod { get; set; } = AccessMethod.Manual;

        public DateTime StartTimeUtc { get; set; }

        public DateTime EndTimeUtc { get; set; }

        public DateTime? ActualEntryTimeUtc { get; set; }

        public DateTime? ActualExitTimeUtc { get; set; }

        public DateTime? CompletedAtUtc { get; set; }

        public decimal Amount { get; set; }

        [Required]
        [MaxLength(3)]
        public string CurrencyCode { get; set; } = "XOF";

        [MaxLength(100)]
        public string? PaymentReference { get; set; }

        [MaxLength(500)]
        public string? CancellationReason { get; set; }

        public DateTime? CancelledAtUtc { get; set; }

        [MaxLength(450)]
        public string? CancelledByUserId { get; set; }

        [MaxLength(500)]
        public string? AccessCredential { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAtUtc { get; set; }

        public DateTime ExpiredAtUtc { get; set; }

        public DateTime NoShowAtUtc { get; set; }

        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        public ReservationHold? Hold { get; set; }
    }
}