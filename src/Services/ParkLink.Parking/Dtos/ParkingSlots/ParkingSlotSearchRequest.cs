using ParkLink.Parking.Enums;
using ParkLink.Shared.Contracts.Enums;

namespace ParkLink.Parking.Dtos.ParkingSlots
{
    public sealed class ParkingSlotSearchRequest
    {
        public Guid? ParkingZoneId { get; set; }

        public Guid? ParkingLotId { get; set; }

        public string? Search { get; set; }

        public ParkingSlotType? SlotType { get; set; }

        public ParkingSlotStatus? Status { get; set; }

        public bool? IsActive { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }
}
