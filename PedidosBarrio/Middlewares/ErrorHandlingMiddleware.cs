
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace PedidosBarrio.Api.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ha ocurrido un error inesperado.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError; // Default
            string message = "Ocurrió un error interno del servidor.";
            object errors = null;

            switch (exception)
            {
                case ApplicationException appException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = appException.Message;
                    break;
                case ValidationException validationException: // <-- Nuevo caso para FluentValidation
                    statusCode = HttpStatusCode.BadRequest;
                    message = "Errores de validación."; // Mensaje general para validación
                                                        // Extraemos los errores de FluentValidation y los formateamos
                    errors = validationException.Errors
                                                .Select(e => new { Field = e.PropertyName, Message = e.ErrorMessage })
                                                .ToList();
                    break;
                // Puedes añadir más tipos de excepciones y códigos de estado aquí
                default:
                    // Para otras excepciones no controladas, se mantiene 500
                    // En un entorno de producción, podrías registrar esta excepción completa
                    // para depuración sin exponer detalles al cliente.
                    break;
            }
            context.Response.StatusCode = (int)statusCode;

            var responseBody = new
            {
                error = message,
                status = (int)statusCode,
                errors
            };

            var result = JsonSerializer.Serialize(responseBody, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Opcional: para usar camelCase en JSON
            });

            return context.Response.WriteAsync(result);
        }
    }

    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}
