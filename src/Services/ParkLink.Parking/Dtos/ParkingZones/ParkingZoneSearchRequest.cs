using ParkLink.Parking.Enums;

namespace ParkLink.Parking.Dtos.ParkingZones
{
    public sealed class ParkingZoneSearchRequest
    {
        public Guid? ParkingLotId { get; set; }

        public string? Search { get; set; }

        public ParkingZoneStatus? Status { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }
}
