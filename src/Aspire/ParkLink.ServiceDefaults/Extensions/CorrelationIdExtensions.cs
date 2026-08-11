using Microsoft.AspNetCore.Builder;
using ParkLink.ServiceDefaults.Middleware;

namespace ParkLink.ServiceDefaults.Extensions
{
    public static class CorrelationIdExtensions
    {
        public static IApplicationBuilder UseParkLinkCorrelationId(
            this IApplicationBuilder app)
        {
            return app.UseMiddleware<CorrelationIdMiddleware>();
        }
    }
}
