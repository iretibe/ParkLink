using ParkLink.Parking.Enums;
using System.ComponentModel.DataAnnotations;

namespace ParkLink.Parking.Dtos.ParkingZones
{
    public sealed class UpdateParkingZoneRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public ParkingZoneStatus Status { get; set; }

        [Range(0, int.MaxValue)]
        public int Capacity { get; set; }
    }
}
