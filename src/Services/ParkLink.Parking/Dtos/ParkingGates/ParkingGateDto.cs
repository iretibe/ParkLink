using ParkLink.Parking.Enums;

namespace ParkLink.Parking.Dtos.ParkingGates
{
    public sealed class ParkingGateDto
    {
        public Guid Id { get; set; }

        public Guid ParkingLotId { get; set; }

        public string ParkingLotName { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public ParkingGateType GateType { get; set; }

        public GateStatus Status { get; set; }

        public string? DeviceIdentifier { get; set; }

        public string? RfidReaderIdentifier { get; set; }

        public string? OcrCameraIdentifier { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }
    }
}
