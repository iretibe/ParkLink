using ParkLink.Users.Enums;

namespace ParkLink.Users.Dtos.Drivers
{
    public sealed class DriverSearchRequest
    {
        public string? Search { get; set; }
        public DriverStatus? Status { get; set; }
        public string? CountryCode { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
