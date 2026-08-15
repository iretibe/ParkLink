using ParkLink.Payment.Enums;

namespace ParkLink.Payment.Providers
{
    public sealed class PaymentProviderResolver : IPaymentProviderResolver
    {
        private readonly IEnumerable<IPaymentProvider> _providers;

        public PaymentProviderResolver(IEnumerable<IPaymentProvider> providers)
        {
            _providers = providers;
        }

        public IPaymentProvider Resolve(PaymentMethod method)
        {
            var providerName = method switch
            {
                PaymentMethod.MobileMoney => "Paystack",
                PaymentMethod.Card => "Paystack",
                PaymentMethod.BankTransfer => "Paystack",
                PaymentMethod.Wallet => "Mock",
                PaymentMethod.Cash => "Mock",
                _ => "Mock"
            };

            return _providers.FirstOrDefault(
                x => x.Name.Equals(providerName, StringComparison.OrdinalIgnoreCase))
                   ?? throw new InvalidOperationException(
                       $"No payment provider is registered for {method}.");
        }
    }
}
