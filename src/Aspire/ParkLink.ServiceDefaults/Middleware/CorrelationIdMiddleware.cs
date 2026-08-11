using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ParkLink.ServiceDefaults.Correlation;
using System.Diagnostics;

namespace ParkLink.ServiceDefaults.Middleware
{
    public sealed class CorrelationIdMiddleware
    {
        private const string HeaderName = "X-Correlation-ID";

        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context,
            ICorrelationContext correlationContext,
            ILogger<CorrelationIdMiddleware> logger)
        {
            var correlationId =
                context.Request.Headers[HeaderName].FirstOrDefault()
                    ?? Activity.Current?.TraceId.ToString()
                    ?? Guid.NewGuid().ToString();

            correlationContext.Set(correlationId);

            context.Response.Headers[HeaderName] = correlationId;

            using var scope =
                logger.BeginScope(
                    new Dictionary<string, object>
                    {
                        ["CorrelationId"] = correlationId
                    });

            await _next(context);
        }
    }
}
