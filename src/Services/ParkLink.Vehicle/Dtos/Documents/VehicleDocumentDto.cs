using ParkLink.Vehicle.Enums;

namespace ParkLink.Vehicle.Dtos.Documents
{
    public class VehicleDocumentDto
    {
        public Guid Id { get; set; }
        public VehicleDocumentType DocumentType { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public string IssuingCountryCode { get; set; } = string.Empty;
        public string? DocumentUrl { get; set; }
        public DateTime? ExpiryDateUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
