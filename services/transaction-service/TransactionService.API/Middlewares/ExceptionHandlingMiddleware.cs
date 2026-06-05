using System.Net;
using Serilog;
using TransactionService.Domain.Exceptions;

namespace TransactionService.API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DomainException ex)
            {
                Log.Warning(ex, "Domain validation error");
                await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception e)
            {
                Log.Error(e, "An unhandled exception occurred");
                var realMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, realMessage);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                Success = false,
                message
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
