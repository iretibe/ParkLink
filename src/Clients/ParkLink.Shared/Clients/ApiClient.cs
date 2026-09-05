using Microsoft.Extensions.Configuration;
using ParkLink.Shared.Providers;
using System.Net.Http.Headers;

namespace ParkLink.Shared.Clients
{
    public class ApiClient
    {
        private readonly IHttpClientFactory _factory;
        private readonly ITokenProvider _tokenProvider;
        private readonly IConfiguration _config;

        public ApiClient(IHttpClientFactory factory,
            ITokenProvider tokenProvider, IConfiguration config)
        {
            _factory = factory;
            _tokenProvider = tokenProvider;
            _config = config;
        }

        public async Task<HttpClient> CreateAsync()
        {
            var token = await _tokenProvider.GetAccessTokenAsync();

            if (string.IsNullOrEmpty(token))
                throw new Exception("Access token is missing. User may not be authenticated.");

            var client = _factory.CreateClient();
            client.BaseAddress = new Uri(_config["ApiSettings:BaseUrl"]!);

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            return client;
        }
    }
}
