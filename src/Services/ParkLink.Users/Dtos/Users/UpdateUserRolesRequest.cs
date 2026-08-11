using System.ComponentModel.DataAnnotations;

namespace ParkLink.Users.Dtos.Users
{
    public sealed class UpdateUserRolesRequest
    {
        [Required]
        public IReadOnlyCollection<string> Roles { get; init; } = Array.Empty<string>();
    }
}
