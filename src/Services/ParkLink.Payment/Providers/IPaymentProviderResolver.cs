using ParkLink.Payment.Enums;

namespace ParkLink.Payment.Providers
{
    public interface IPaymentProviderResolver
    {
        IPaymentProvider Resolve(PaymentMethod method);
    }
}
