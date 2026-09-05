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
                context.Request.Headers[HeaderName].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(correlationId)) 
            { 
                correlationId = Activity.Current?.TraceId.ToString() 
                    ?? Guid.NewGuid().ToString("N"); 
            }

            correlationContext.Set(correlationId);

            context.Response.OnStarting(() => 
            { 
                context.Response.Headers[HeaderName] = correlationId; 

                return Task.CompletedTask; 
            });

            using var scope = logger.BeginScope(
                new Dictionary<string, object> 
                { 
                    ["CorrelationId"] = correlationId 
                }
            );

            //await _next(context);

            try
            {
                await _next(context);
            }
            catch (TaskCanceledException ex)
            {
                logger.LogWarning(ex, "Request canceled. RequestAborted={RequestAborted}", context.RequestAborted.IsCancellationRequested);
                throw;
            }
        }
    }
}
