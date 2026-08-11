namespace ParkLink.Users.Dtos.Documents
{
    public class UpdateUserDocumentRequest
    {
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public string IssuingCountryCode { get; set; } = string.Empty;
    }
}
