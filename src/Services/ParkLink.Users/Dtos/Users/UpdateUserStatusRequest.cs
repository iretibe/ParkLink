using System.ComponentModel.DataAnnotations;

namespace ParkLink.Users.Dtos.Users
{
    public sealed class UpdateUserStatusRequest
    {
        [Required]
        public bool IsActive { get; init; }
    }
}
