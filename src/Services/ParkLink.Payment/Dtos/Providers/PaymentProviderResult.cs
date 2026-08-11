namespace ParkLink.Payment.Dtos.Providers
{
    public sealed record PaymentProviderResult(
        bool Success,
        string? ProviderReference,
        string? PaymentReference,
        string? AuthorizationUrl,
        string? ErrorMessage
    );
}
