using ParkLink.Payment.Dtos.Providers;
using ParkLink.Payment.Responses;

namespace ParkLink.Payment.Providers
{
    public sealed class PaystackPaymentProvider : IPaymentProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaystackPaymentProvider> _logger;

        public string Name => "Paystack";

        public PaystackPaymentProvider(HttpClient httpClient,
            IConfiguration configuration, ILogger<PaystackPaymentProvider> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<PaymentProviderResult> InitializePaymentAsync(
            PaymentProviderRequest request, CancellationToken cancellationToken = default)
        {
            var secretKey = _configuration["Payment:Paystack:SecretKey"];

            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new InvalidOperationException(
                    "Paystack secret key has not been configured.");
            }

            var amountInKobo = Convert.ToInt64(request.Amount * 100);

            var payload = new
            {
                email = request.CustomerEmail,
                amount = amountInKobo,
                currency = request.CurrencyCode,
                reference = $"PL-{request.PaymentId:N}",
                callback_url = request.CallbackUrl
            };

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "transaction/initialize");

            httpRequest.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", secretKey);

            httpRequest.Content = JsonContent.Create(payload);

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);

                _logger.LogError("Paystack initialization failed: {Error}", error);

                return PaymentProviderResult.Failed(
                    "Payment provider initialization failed.");
            }

            var result =
                await response.Content.ReadFromJsonAsync<PaystackInitializeResponse>(
                    cancellationToken: cancellationToken);

            if (result?.Data == null)
            {
                return PaymentProviderResult.Failed(
                    "Invalid response received from Paystack.");
            }

            return PaymentProviderResult.Successful(
                paymentReference: result.Data.Reference,
                providerReference: result.Data.Reference,
                authorizationUrl: result.Data.AuthorizationUrl
            );
        }

        public async Task<PaymentProviderResult> RefundPaymentAsync(string providerReference, decimal amount, CancellationToken cancellationToken = default)
        {
            var secretKey = _configuration["Payment:Paystack:SecretKey"];

            using var request = new HttpRequestMessage(HttpMethod.Post, "refund");

            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", secretKey);

            request.Content = JsonContent.Create(
                new
                {
                    transaction = providerReference,
                    amount = Convert.ToInt64(amount * 100)
                });

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return PaymentProviderResult.Failed("Payment refund failed.");
            }

            return PaymentProviderResult.Successful(
                paymentReference: providerReference,
                providerReference: providerReference
            );
        }

        public async Task<PaymentProviderResult> VerifyPaymentAsync(string providerReference, CancellationToken cancellationToken = default)
        {
            var secretKey = _configuration["Payment:Paystack:SecretKey"];

            using var request = new HttpRequestMessage(HttpMethod.Get,
                $"transaction/verify/{providerReference}");

            request.Headers.Authorization = new 
                System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", secretKey);

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return PaymentProviderResult.Failed("Unable to verify payment with Paystack.");
            }

            var result = 
                await response.Content.ReadFromJsonAsync<PaystackVerifyResponse>(
                    cancellationToken: cancellationToken);

            if (result?.Data == null)
            {
                return PaymentProviderResult.Failed(
                    "Invalid payment verification response.");
            }

            if (!string.Equals(result.Data.Status, "success", StringComparison.OrdinalIgnoreCase))
            {
                return PaymentProviderResult.Failed(
                    result.Data.Status ?? "Payment verification failed.");
            }

            return PaymentProviderResult.Successful(
                paymentReference: result.Data.Reference,
                providerReference: result.Data.Reference
            );
        }
    }
}
