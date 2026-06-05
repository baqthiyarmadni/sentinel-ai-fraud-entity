using Serilog;

namespace TransactionService.API.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = context.Items["corrleationId"]?.ToString() ?? "N/A";

            Log.Information(
                "Request Started | " +
                "CorrelationId:{CorrelationId} " +
                "Method:{Method} " +
                "Path:{Path}",
                correlationId,
                context.Request.Method,
                context.Request.Path);

            await _next(context);

            Log.Information(
                "Request Completed | " +
                "CorrelationId:{CorrelationId} " +
                "StatusCode:{StatusCode}",
                correlationId,
                context.Response.StatusCode);
        }
    }
}
