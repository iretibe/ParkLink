using ParkLink.Parking.Enums;

namespace ParkLink.Parking.Dtos.ParkingLots
{
    public sealed class ParkingGateSummaryDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public ParkingGateType GateType { get; set; }

        public GateStatus Status { get; set; }

        public bool IsActive { get; set; }

        public string? DeviceIdentifier { get; set; }

        public string? RfidReaderIdentifier { get; set; }

        public string? OcrCameraIdentifier { get; set; }
    }
}
