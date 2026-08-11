using ParkLink.Parking.Enums;
using ParkLink.Shared.Contracts.Enums;

namespace ParkLink.Parking.Dtos.ParkingSlots
{
    public sealed class ParkingSlotDto
    {
        public Guid Id { get; set; }

        public Guid ParkingZoneId { get; set; }

        public Guid ParkingLotId { get; set; }

        public string ParkingLotName { get; set; } = string.Empty;

        public string ParkingZoneName { get; set; } = string.Empty;

        public string SlotNumber { get; set; } = string.Empty;

        public ParkingSlotType SlotType { get; set; }

        public ParkingSlotStatus Status { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }

        public DateTime? LastStatusChangedAtUtc { get; set; }
    }
}
