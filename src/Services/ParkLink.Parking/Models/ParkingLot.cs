using ParkLink.Parking.Enums;
using System.ComponentModel.DataAnnotations;

namespace ParkLink.Parking.Models
{
    public class ParkingLot
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(1000)]
        public string? Description { get; set; }
        [Required]
        [MaxLength(100)]
        public string CountryCode { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;
        [MaxLength(500)]
        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public ParkingLotStatus Status { get; set; } = ParkingLotStatus.Draft;
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
        public ICollection<ParkingZone> Zones { get; set; } = new List<ParkingZone>();
        public ICollection<ParkingGate> Gates { get; set; } = new List<ParkingGate>();
    }
}
