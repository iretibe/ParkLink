using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;

namespace ParkLink.Shared.Providers
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _http;
        private readonly AuthenticationStateProvider _auth;

        public TokenProvider(IHttpContextAccessor http,
            AuthenticationStateProvider auth)
        {
            _http = http;
            _auth = auth;
        }

        public async Task<string?> GetAccessTokenAsync()
        {
            // Try HttpContext (first request only)
            var ctx = _http.HttpContext;
            if (ctx != null)
            {
                var token = await ctx.GetTokenAsync("access_token");
                if (!string.IsNullOrEmpty(token))
                    return token;
            }

            // Fallback to claims (Blazor circuit)
            var state = await _auth.GetAuthenticationStateAsync();
            return state.User?.FindFirst("access_token")?.Value;
        }
    }
}
