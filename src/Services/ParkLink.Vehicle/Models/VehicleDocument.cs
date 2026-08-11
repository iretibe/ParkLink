using ParkLink.Vehicle.Enums;

namespace ParkLink.Vehicle.Models
{
    public class VehicleDocument
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
        public Vehicle Vehicle { get; set; } = default!;
    }
}
