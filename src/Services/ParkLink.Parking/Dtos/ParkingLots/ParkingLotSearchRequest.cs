using ParkLink.Parking.Enums;

namespace ParkLink.Parking.Dtos.ParkingLots
{
    public sealed class ParkingLotSearchRequest
    {
        public string? Search { get; set; }

        public string? CountryCode { get; set; }

        public string? City { get; set; }

        public ParkingLotStatus? Status { get; set; }

        public bool? IsActive { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }
}
