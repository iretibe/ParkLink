using System.Security.Cryptography;
using System.Text;

namespace ParkLink.Payment.Services
{
    public sealed class PaystackWebhookValidator : IPaystackWebhookValidator
    {
        private readonly IConfiguration _configuration;

        public PaystackWebhookValidator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool Validate(string payload, string signature)
        {
            if (string.IsNullOrWhiteSpace(signature))
            {
                return false;
            }

            var secretKey = _configuration["Payment:Paystack:SecretKey"];

            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new InvalidOperationException(
                    "Paystack secret key has not been configured.");
            }

            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secretKey));

            var hash = Convert.ToHexString(
                hmac.ComputeHash(Encoding.UTF8.GetBytes(payload))).ToLowerInvariant();

            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(hash),
                Encoding.UTF8.GetBytes(signature.ToLowerInvariant()));
        }
    }
}
