using ParkLink.Users.Enums;

namespace ParkLink.Users.Dtos.Drivers
{
    public sealed class DriverListItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string CountryCode { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DriverStatus DriverStatus { get; set; }
        public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
    }
}
