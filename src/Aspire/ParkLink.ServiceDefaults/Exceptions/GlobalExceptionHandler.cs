using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ParkLink.ServiceDefaults.Exceptions
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,
            Exception exception, CancellationToken cancellationToken)
        {
            var problemDetails = new ProblemDetails
            {
                Status = GetStatusCode(exception),
                Title = GetTitle(exception),
                Detail = GetDetail(exception),
                Instance = httpContext.Request.Path
            };

            problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

            if (httpContext.Request.Headers.TryGetValue("X-Correlation-ID",
                    out var correlationId))
            {
                problemDetails.Extensions["correlationId"] = correlationId.ToString();
            }

            _logger.LogError(
                exception,
                "Unhandled exception. TraceId: {TraceId}, CorrelationId: {CorrelationId}",
                httpContext.TraceIdentifier,
                correlationId.ToString());

            httpContext.Response.StatusCode = problemDetails.Status!.Value;

            await httpContext.Response.WriteAsJsonAsync(problemDetails,
                cancellationToken);

            return true;
        }

        private static int GetStatusCode(Exception exception)
        {
            return exception switch
            {
                ArgumentException => StatusCodes.Status400BadRequest,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                InvalidOperationException => StatusCodes.Status409Conflict,
                UnauthorizedAccessException => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError
            };
        }

        private static string GetTitle(Exception exception)
        {
            return exception switch
            {
                ArgumentException => "Bad Request",
                KeyNotFoundException => "Resource Not Found",
                InvalidOperationException => "Conflict",
                UnauthorizedAccessException => "Forbidden",
                _ => "An unexpected error occurred"
            };
        }

        private static string GetDetail(Exception exception)
        {
            return exception switch
            {
                ArgumentException => exception.Message,
                KeyNotFoundException => exception.Message,
                InvalidOperationException => exception.Message,
                UnauthorizedAccessException => exception.Message,
                _ =>
                    "An unexpected error occurred while processing your request."
            };
        }
    }
}
