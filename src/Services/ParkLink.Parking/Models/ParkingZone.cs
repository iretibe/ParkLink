using ParkLink.Parking.Enums;
using System.ComponentModel.DataAnnotations;

namespace ParkLink.Parking.Models
{
    public class ParkingZone
    {
        public Guid Id { get; set; }
        public Guid ParkingLotId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(500)]
        public string? Description { get; set; }
        public ParkingZoneStatus Status { get; set; } = ParkingZoneStatus.Active;
        public int Capacity { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
        public ParkingLot ParkingLot { get; set; } = default!;
        public ICollection<ParkingSlot> Slots { get; set; } = new List<ParkingSlot>();
    }
}
