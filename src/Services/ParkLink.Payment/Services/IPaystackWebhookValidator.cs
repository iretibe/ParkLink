namespace ParkLink.Payment.Services
{
    public interface IPaystackWebhookValidator
    {
        bool Validate(string payload, string signature);
    }
}
