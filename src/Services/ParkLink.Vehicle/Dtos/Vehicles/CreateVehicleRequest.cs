using ParkLink.Vehicle.Enums;
using System.ComponentModel.DataAnnotations;

namespace ParkLink.Vehicle.Dtos.Vehicles
{
    public class CreateVehicleRequest
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
        public ICollection<VehicleDocumentRequest> Documents { get; set; }
            = new List<VehicleDocumentRequest>();
    }

    public class VehicleDocumentRequest
    {
        public Guid Id { get; set; }
        public Guid VehicleId { get; set; }
        public VehicleDocumentType DocumentType { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public string IssuingCountryCode { get; set; } = string.Empty;
        public string? DocumentUrl { get; set; }
        public DateTime? ExpiryDateUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
        public CreateVehicleRequest Vehicle { get; set; } = default!;
    }
}
