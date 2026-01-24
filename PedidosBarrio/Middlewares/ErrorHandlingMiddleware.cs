using FluentValidation;
using System.Net;
using System.Text.Json;

namespace PedidosBarrio.Api.Middlewares
{
    /// <summary>
    /// Middleware centralizado para:
    /// 1. Capturar TODAS las excepciones no controladas
    /// 2. Loguear los errores
    /// 3. Retornar respuestas consistentes
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISimpleLogger _logger;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
            _logger = new SimpleFileLogger("Logs");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // ✅ AQUÍ SE LOGUEAN TODOS LOS ERRORES
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            string message = "Ocurrió un error interno del servidor.";
            object errors = null;
            string logLevel = "ERROR";
            string exceptionType = exception.GetType().Name;

            // Determinar tipo de error y loguear
            switch (exception)
            {
                case ValidationException validationException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = "Errores de validación.";
                    errors = validationException.Errors
                        .Select(e => new { Field = e.PropertyName, Message = e.ErrorMessage })
                        .ToList();
                    logLevel = "WARNING";
                    exceptionType = "ValidationException";

                    // ✅ LOGUEAR ERRORES DE VALIDACIÓN
                    var validationErrors = string.Join("; ", 
                        validationException.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                    await _logger.LogAsync(logLevel, "VALIDATION_ERROR", 
                        $"Validación fallida en {context.Request.Path}\nErrores: {validationErrors}");
                    break;

                case ApplicationException appException:
                    // ✅ DISTINGUIR ENTRE ERRORES DE APLICACIÓN Y BASE DE DATOS
                    if (IsDataBaseException(appException))
                    {
                        // Error de BD = 500
                        statusCode = HttpStatusCode.InternalServerError;
                        message = "Error en la base de datos. Por favor, intente más tarde.";
                        logLevel = "ERROR";
                        exceptionType = "DatabaseException";

                        await _logger.LogAsync(logLevel, "DATABASE_ERROR",
                            $"Error de base de datos en {context.Request.Path}\nMensaje: {appException.Message}", appException);
                    }
                    else
                    {
                        // Error de aplicación = 400
                        statusCode = HttpStatusCode.BadRequest;
                        message = appException.Message;
                        logLevel = "WARNING";
                        exceptionType = "ApplicationException";

                        await _logger.LogAsync(logLevel, "APPLICATION_ERROR",
                            $"Error de aplicación en {context.Request.Path}\nMensaje: {appException.Message}");
                    }
                    break;

                case ArgumentNullException argNullEx:
                    statusCode = HttpStatusCode.BadRequest;
                    message = "Argumento nulo o inválido.";
                    logLevel = "WARNING";
                    exceptionType = "ArgumentNullException";

                    // ✅ LOGUEAR ARGUMENTOS NULOS
                    await _logger.LogAsync(logLevel, "ARGUMENT_NULL_ERROR",
                        $"Argumento nulo en {context.Request.Path}\nDetalle: {argNullEx.Message}", argNullEx);
                    break;

                case KeyNotFoundException keyNotFoundEx:
                    statusCode = HttpStatusCode.NotFound;
                    message = "Recurso no encontrado.";
                    logLevel = "WARNING";
                    exceptionType = "KeyNotFoundException";

                    // ✅ LOGUEAR NO ENCONTRADO
                    await _logger.LogAsync(logLevel, "NOT_FOUND_ERROR",
                        $"Recurso no encontrado en {context.Request.Path}");
                    break;

                default:
                    // ✅ LOGUEAR EXCEPCIONES NO CONTROLADAS CON STACK TRACE
                    await _logger.LogAsync("ERROR", "UNHANDLED_EXCEPTION",
                        $"Excepción no controlada de tipo {exceptionType} en {context.Request.Path}\n" +
                        $"Método: {context.Request.Method} {context.Request.Path}\n" +
                        $"IP: {context.Connection.RemoteIpAddress}\n" +
                        $"Mensaje: {exception.Message}",
                        exception);
                    break;
            }

                context.Response.StatusCode = (int)statusCode;

                var responseBody = new
                {
                    error = message,
                    status = (int)statusCode,
                    timestamp = DateTime.UtcNow,
                    path = context.Request.Path,
                    method = context.Request.Method,
                    exceptionType = exceptionType,
                    detail = GetDeepestExceptionMessage(exception),
                    errors
                };

                var result = JsonSerializer.Serialize(responseBody, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await context.Response.WriteAsync(result);
            }

            private string GetDeepestExceptionMessage(Exception ex)
            {
                if (ex.InnerException == null) return ex.Message;
                return GetDeepestExceptionMessage(ex.InnerException);
            }

            /// <summary>
            /// Detecta si una excepción es relacionada con base de datos
            /// </summary>
        private bool IsDataBaseException(ApplicationException appException)
        {
            if (appException.InnerException == null)
                return false;

            var innerException = appException.InnerException;
            var exceptionTypeName = innerException.GetType().FullName ?? "";

            // Verificar si es excepción de BD
            return exceptionTypeName.Contains("Npgsql") ||
                   exceptionTypeName.Contains("SqlException") ||
                   exceptionTypeName.Contains("Oracle") ||
                   exceptionTypeName.Contains("MySQL") ||
                   exceptionTypeName.Contains("DataException") ||
                   innerException.Message.Contains("constraint") ||
                   innerException.Message.Contains("duplicate") ||
                   innerException.Message.Contains("connection") ||
                   innerException.Message.Contains("timeout");
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
