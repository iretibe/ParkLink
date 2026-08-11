using ParkLink.Parking.Enums;
using ParkLink.Shared.Contracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ParkLink.Parking.Models
{
    public class ParkingSlot
    {
        public Guid Id { get; set; }
        public Guid ParkingZoneId { get; set; }
        [Required]
        [MaxLength(50)]
        public string SlotNumber { get; set; } = string.Empty;
        public ParkingSlotType SlotType { get; set; } = ParkingSlotType.Standard;
        public ParkingSlotStatus Status { get; set; } = ParkingSlotStatus.Available;
        public bool IsActive { get; set; } = true;
        public string? Description { get; set; }
        // Used for optimistic concurrency.
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
        public DateTime? LastStatusChangedAtUtc { get; set; }
        public ParkingZone ParkingZone { get; set; } = default!;
    }
}
