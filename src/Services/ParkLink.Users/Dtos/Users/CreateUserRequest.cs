using System.ComponentModel.DataAnnotations;

namespace ParkLink.Users.Dtos.Users
{
    public sealed class CreateUserRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; init; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string UserName { get; init; } = string.Empty;

        [Required]
        public string FirstName { get; init; } = string.Empty;

        [Required]
        public string LastName { get; init; } = string.Empty;

        public string? MiddleName { get; init; }

        [Required]
        public string CountryCode { get; init; } = string.Empty;

        public string PreferredLanguage { get; init; } = "en";

        public string TimeZoneId { get; init; } = "UTC";

        [Required]
        [MinLength(8)]
        public string Password { get; init; } = string.Empty;

        public IReadOnlyCollection<string> Roles { get; init; } = Array.Empty<string>();
    }
}
