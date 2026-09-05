using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ParkLink.Shared.Security
{
    public class OidcWarmupService : IHostedService
    {
        private readonly IHttpClientFactory _factory;
        private readonly IConfiguration _config;
        private readonly ILogger<OidcWarmupService> _logger;

        public OidcWarmupService(
            IHttpClientFactory factory,
            IConfiguration config,
            ILogger<OidcWarmupService> logger)
        {
            _factory = factory;
            _config = config;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var authority = _config["MawumsOidc:AuthorityUrl"];

            var url = $"{authority}/.well-known/openid-configuration";

            try
            {
                var client = _factory.CreateClient();
                var res = await client.GetAsync(url, cancellationToken);

                if (res.IsSuccessStatusCode)
                    _logger.LogInformation("OIDC authority warm-up successful");
                else
                    _logger.LogWarning("OIDC authority warm-up returned {code}", res.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "OIDC authority warm-up failed (will retry automatically later)");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
