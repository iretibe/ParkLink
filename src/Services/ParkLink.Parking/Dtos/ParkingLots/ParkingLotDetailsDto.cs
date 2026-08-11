using ParkLink.Parking.Enums;

namespace ParkLink.Parking.Dtos.ParkingLots
{
    public sealed class ParkingLotDetailsDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string CountryCode { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string? Address { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public ParkingLotStatus Status { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }

        public int ZoneCount { get; set; }

        public int GateCount { get; set; }

        public int TotalSlotCount { get; set; }

        public int AvailableSlotCount { get; set; }

        public int ReservedSlotCount { get; set; }

        public int OccupiedSlotCount { get; set; }

        public int MaintenanceSlotCount { get; set; }

        public int DisabledSlotCount { get; set; }

        public IReadOnlyCollection<ParkingZoneSummaryDto> Zones { get; set; }
            = Array.Empty<ParkingZoneSummaryDto>();

        public IReadOnlyCollection<ParkingGateSummaryDto> Gates { get; set; }
            = Array.Empty<ParkingGateSummaryDto>();
    }
}
