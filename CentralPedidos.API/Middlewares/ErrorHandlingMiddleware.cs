using CentralPedidos.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace CentralPedidos.API.Middlewares
{
    public class ErrorHandlingMiddleware(RequestDelegate requestDelegate, ILogger<ErrorHandlingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await requestDelegate(context);
            }
            catch (DomainException ex)
            {
                logger.LogWarning(ex, "Regra de negócio violada: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex.Message, HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ocorreu um erro interno inesperado.");
                await HandleExceptionAsync(context, "Ocorreu um erro interno no servidor.", HttpStatusCode.InternalServerError);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, string message, HttpStatusCode statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new { error = message };
            string jsonResponse = JsonSerializer.Serialize(response);

            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
