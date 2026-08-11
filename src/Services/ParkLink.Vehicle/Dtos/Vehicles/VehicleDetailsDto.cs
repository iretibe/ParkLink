using ParkLink.Vehicle.Dtos.Documents;
using ParkLink.Vehicle.Enums;

namespace ParkLink.Vehicle.Dtos.Vehicles
{
    public class VehicleDetailsDto
    {
        public Guid Id { get; set; }
        public string OwnerId { get; set; } = string.Empty;
        public string LicensePlateNumber { get; set; } = string.Empty;
        public string? VIN { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int? Year { get; set; }
        public string? Color { get; set; }
        public VehicleType VehicleType { get; set; }
        public VehicleStatus Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public DateTime? VerifiedAtUtc { get; set; }
        public string? VerifiedByUserId { get; set; }
        public DateTime? SuspendedAtUtc { get; set; }
        public string? SuspendedByUserId { get; set; }
        public string? StatusReason { get; set; }
        public List<VehicleDocumentDto> Documents { get; set; } = [];
    }
}
