using Microsoft.AspNetCore.Identity;

namespace ParkLink.Users.Exceptions
{
    public sealed class IdentityOperationException : Exception
    {
        public IReadOnlyCollection<IdentityError> Errors { get; }

        public IdentityOperationException(IEnumerable<IdentityError> errors)
            : base("The identity operation failed.")
        {
            Errors = errors.ToArray();
        }
    }
}
