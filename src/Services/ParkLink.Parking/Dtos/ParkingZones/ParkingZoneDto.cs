using ParkLink.Parking.Enums;

namespace ParkLink.Parking.Dtos.ParkingZones
{
    public sealed class ParkingZoneDto
    {
        public Guid Id { get; set; }

        public Guid ParkingLotId { get; set; }

        public string ParkingLotName { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ParkingZoneStatus Status { get; set; }

        public int Capacity { get; set; }

        public int TotalSlotCount { get; set; }

        public int AvailableSlotCount { get; set; }

        public int ReservedSlotCount { get; set; }

        public int OccupiedSlotCount { get; set; }

        public int MaintenanceSlotCount { get; set; }

        public int DisabledSlotCount { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }
    }
}
