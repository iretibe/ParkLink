using Microsoft.AspNetCore.Identity;
using ParkLink.Identity.Enums;

namespace ParkLink.Identity.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Personal Information
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public DateTime? DateOfBirth { get; set; }

        // Localization (en, fr)
        public string PreferredLanguage { get; set; } = "en";

        // Country Code (GH, BJ, CI, TG, SN, NG)
        public string CountryCode { get; set; } = "GH";

        // Timezone (Africa/Accra, Africa/Lagos, Africa/Abidjan, Africa/Cotonou)
        public string TimeZoneId { get; set; } = "Africa/Accra";

        // Driver's Profile
        public bool IsDriver { get; set; } = true;
        public DriverVerificationStatus VerificationStatus { get; set; }
            = DriverVerificationStatus.Pending;

        // Account Status
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAtUtc { get; set; }

        // Audit
        public DateTime? UpdatedAtUtc { get; set; }

        public ICollection<UserDocument> Documents { get; set; }
            = new List<UserDocument>();
    }
}
