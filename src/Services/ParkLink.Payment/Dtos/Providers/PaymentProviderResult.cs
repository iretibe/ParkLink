namespace ParkLink.Payment.Dtos.Providers
{
    public sealed class PaymentProviderResult
    {
        public bool Success { get; init; }
        public bool RequiresAction { get; init; }
        public string? PaymentReference { get; init; }
        public string? ProviderReference { get; init; }
        public string? AuthorizationUrl { get; init; }
        public string? FailureReason { get; init; }
        public DateTime? ProcessedAtUtc { get; init; }

        public static PaymentProviderResult Successful(
            string? paymentReference = null,
            string? providerReference = null,
            string? authorizationUrl = null)
        {
            return new PaymentProviderResult
            {
                Success = true,
                PaymentReference = paymentReference,
                ProviderReference = providerReference,
                AuthorizationUrl = authorizationUrl,
                ProcessedAtUtc = DateTime.UtcNow
            };
        }

        public static PaymentProviderResult Failed(string reason)
        {
            return new PaymentProviderResult
            {
                Success = false,
                FailureReason = reason,
                ProcessedAtUtc = DateTime.UtcNow
            };
        }
    }
}
