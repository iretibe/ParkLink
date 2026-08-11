using ParkLink.Parking.Enums;

namespace ParkLink.Parking.Dtos.ParkingLots
{
    public sealed class ParkingZoneSummaryDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public ParkingZoneStatus Status { get; set; }

        public int Capacity { get; set; }

        public int TotalSlotCount { get; set; }

        public int AvailableSlotCount { get; set; }

        public int OccupiedSlotCount { get; set; }
    }
}
