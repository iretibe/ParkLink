using ParkLink.Parking.Enums;
using System.ComponentModel.DataAnnotations;

namespace ParkLink.Parking.Dtos.ParkingGates
{
    public sealed class UpdateParkingGateRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public ParkingGateType GateType { get; set; }

        public GateStatus Status { get; set; }

        [MaxLength(100)]
        public string? DeviceIdentifier { get; set; }

        [MaxLength(100)]
        public string? RfidReaderIdentifier { get; set; }

        [MaxLength(100)]
        public string? OcrCameraIdentifier { get; set; }

        public bool IsActive { get; set; }
    }
}
