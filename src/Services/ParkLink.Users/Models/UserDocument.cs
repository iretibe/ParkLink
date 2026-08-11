using ParkLink.Users.Enums;

namespace ParkLink.Users.Models
{
    public class UserDocument
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        // Document Type (Passport, Driver's License, National ID, etc.)
        public DocumentType DocumentType { get; set; }

        // Country that issued the document.
        public string IssuingCountryCode { get; set; } = string.Empty;

        // Document Details (e.g., Passport Number, License Number, etc.)
        public string DocumentNumber { get; set; } = string.Empty;
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }

        // Verification Status (Pending, Verified, Rejected)
        public DocumentVerificationStatus VerificationStatus { get; set; }
            = DocumentVerificationStatus.Pending;

        public DateTime? VerifiedAtUtc { get; set; }

        public string? VerifiedByUserId { get; set; }

        // Document Files
        public string? FrontImageUrl { get; set; }
        public string? BackImageUrl { get; set; }

        // Audit
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
