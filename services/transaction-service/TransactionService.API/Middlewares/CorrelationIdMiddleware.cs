namespace TransactionService.API.Middlewares
{
    public class CorrelationIdMiddleware
    {
        private const string HeaderName = "X-Correlation-Id";

        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var corrleationId = context.Request.Headers
                .TryGetValue(HeaderName, out var headerValue)
                ? headerValue.ToString()
                : Guid.NewGuid().ToString();

            context.Items["corrleationId"] = corrleationId;

            context.Response.Headers[HeaderName] = corrleationId;

            await _next(context);
        }
    }
}
