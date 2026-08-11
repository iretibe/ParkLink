using ParkLink.Vehicle.Enums;

namespace ParkLink.Vehicle.Dtos.Vehicles
{
    public class VehicleListItemDto
    {
        public Guid Id { get; set; }
        public string OwnerId { get; set; } = string.Empty;
        public string LicensePlateNumber { get; set; } = string.Empty;
        public string? VIN { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int? Year { get; set; }
        public string? Color { get; set; } = string.Empty;
        public VehicleType VehicleType { get; set; }
        public VehicleStatus Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
