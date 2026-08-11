using ParkLink.Users.Dtos.Documents;
using ParkLink.Users.Enums;

namespace ParkLink.Users.Dtos.Drivers
{
    public sealed class DriverDetailsDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string PreferredLanguage { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string TimeZoneId { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DriverStatus DriverStatus { get; set; }
        public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
        public IReadOnlyList<UserDocumentDto> Documents { get; set; } = Array.Empty<UserDocumentDto>();
    }
}
