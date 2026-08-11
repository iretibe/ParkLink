using ParkLink.Vehicle.Enums;

namespace ParkLink.Vehicle.Dtos.Vehicles
{
    public class VehicleSearchRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Search { get; set; }
        public VehicleStatus? Status { get; set; }
        public VehicleType? VehicleType { get; set; }
        public string? OwnerId { get; set; }
        public bool? IsActive { get; set; }
        public string? CountryCode { get; set; }
    }
}
