using ParkLink.Payment.Dtos.Providers;

namespace ParkLink.Payment.Providers
{
    public interface IPaymentProvider
    {
        string Name { get; }

        Task<PaymentProviderResult> InitializePaymentAsync(
            PaymentProviderRequest request,
            CancellationToken cancellationToken = default);
        Task<PaymentProviderResult> VerifyPaymentAsync(
            string providerReference,
            CancellationToken cancellationToken = default);
        Task<PaymentProviderResult> RefundPaymentAsync(
            string providerReference, decimal amount,
            CancellationToken cancellationToken = default);
    }
}
