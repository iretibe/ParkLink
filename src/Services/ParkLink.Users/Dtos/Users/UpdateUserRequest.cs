using System.ComponentModel.DataAnnotations;

namespace ParkLink.Users.Dtos.Users
{
    public sealed class UpdateUserRequest
    {
        [Required]
        public string FirstName { get; init; } = string.Empty;

        [Required]
        public string LastName { get; init; } = string.Empty;

        public string? MiddleName { get; init; }

        [Required]
        public string CountryCode { get; init; } = string.Empty;

        public string PreferredLanguage { get; init; } = "en";

        public string TimeZoneId { get; init; } = "UTC";
    }
}
