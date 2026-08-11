namespace ParkLink.Users.Dtos.Users
{
    public sealed class UserDto
    {
        public string Id { get; init; } = string.Empty;
        public string? UserName { get; init; }
        public string? Email { get; init; }
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public string? MiddleName { get; init; }
        public string? CountryCode { get; init; }
        public string? PreferredLanguage { get; init; }
        public string? TimeZoneId { get; init; }
        public bool EmailConfirmed { get; init; }
        public bool IsActive { get; init; }
        public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();
        public DateTimeOffset? CreatedAt { get; init; }
    }
}
