using ParkLink.Vehicle.Enums;
using System.ComponentModel.DataAnnotations;

namespace ParkLink.Vehicle.Models
{
    public class Vehicle
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(450)]
        public string OwnerId { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LicensePlateNumber { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? VIN { get; set; }

        [Required]
        [MaxLength(100)]
        public string Make { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Model { get; set; } = string.Empty;

        public int? Year { get; set; }

        [MaxLength(50)]
        public string? Color { get; set; }

        public VehicleType VehicleType { get; set; }

        public VehicleStatus Status { get; set; }
            = VehicleStatus.Pending;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; }
            = DateTime.UtcNow;

        public DateTime? UpdatedAtUtc { get; set; }

        public DateTime? VerifiedAtUtc { get; set; }

        public string? VerifiedByUserId { get; set; }

        public DateTime? SuspendedAtUtc { get; set; }

        public string? SuspendedByUserId { get; set; }

        [MaxLength(500)]
        public string? StatusReason { get; set; }

        // Optimistic concurrency
        [Timestamp]
        public byte[] RowVersion { get; set; } = [];

        public ICollection<VehicleDocument> Documents { get; set; }
            = new List<VehicleDocument>();
    }
}
