using ParkLink.Payment.Dtos.Providers;

namespace ParkLink.Payment.Providers
{
    public sealed class MockPaymentProvider : IPaymentProvider
    {
        public string Name => "Mock";

        public Task<PaymentProviderResult> InitializePaymentAsync(
            PaymentProviderRequest request, 
            CancellationToken cancellationToken = default)
        {
            var reference = $"MOCK-{Guid.NewGuid():N}".ToUpperInvariant();

            return Task.FromResult(
                PaymentProviderResult.Successful(
                    paymentReference: reference,
                    providerReference: reference
                )
            );
        }

        public Task<PaymentProviderResult> RefundPaymentAsync(
            string providerReference, decimal amount, 
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                PaymentProviderResult.Successful(
                    paymentReference: providerReference,
                    providerReference: providerReference
                )
            );
        }

        public Task<PaymentProviderResult> VerifyPaymentAsync(
            string providerReference, 
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                PaymentProviderResult.Successful(
                    paymentReference: providerReference,
                    providerReference: providerReference
                )
            );
        }
    }
}
