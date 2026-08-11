using ParkLink.Vehicle.Enums;
using System.ComponentModel.DataAnnotations;

namespace ParkLink.Vehicle.Dtos.Vehicles
{
    public class UpdateVehicleRequest
    {
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

        [Range(1900, 2100)]
        public int? Year { get; set; }

        [MaxLength(50)]
        public string? Color { get; set; }

        public VehicleType VehicleType { get; set; }
    }
}
