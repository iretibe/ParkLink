using ParkLink.Parking.Enums;
using System.ComponentModel.DataAnnotations;

namespace ParkLink.Parking.Models
{
    public class ParkingGate
    {
        public Guid Id { get; set; }
        public Guid ParkingLotId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public ParkingGateType GateType { get; set; }
        public GateStatus Status { get; set; } = GateStatus.Offline;
        [MaxLength(100)]
        public string? DeviceIdentifier { get; set; }
        [MaxLength(100)]
        public string? RfidReaderIdentifier { get; set; }
        [MaxLength(100)]
        public string? OcrCameraIdentifier { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
        public ParkingLot ParkingLot { get; set; } = default!;
    }
}
