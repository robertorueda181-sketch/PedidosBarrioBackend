using System.Diagnostics;

namespace PedidosBarrio.Api.Middlewares
{
    // Logger simple sin dependencias externas - para usar como fallback
    public interface ISimpleLogger
    {
        Task LogAsync(string level, string category, string message, Exception exception = null);
    }

    public class SimpleFileLogger : ISimpleLogger
    {
        private readonly string _logDirectory;
        private readonly object _lockObject = new object();

        public SimpleFileLogger(string logDirectory = "Logs")
        {
            _logDirectory = logDirectory;
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        public async Task LogAsync(string level, string category, string message, Exception exception = null)
        {
            try
            {
                var logFileName = Path.Combine(_logDirectory, $"log_{DateTime.UtcNow:yyyy-MM-dd}.txt");
                var logMessage = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}] [{level}] [{category}]\nMessage: {message}";
                
                if (exception != null)
                {
                    logMessage += $"\nException: {exception.Message}\nStackTrace: {exception.StackTrace}";
                }
                logMessage += "\n" + new string('-', 80) + "\n";

                lock (_lockObject)
                {
                    File.AppendAllText(logFileName, logMessage, System.Text.Encoding.UTF8);
                }

                await Task.CompletedTask;
            }
            catch
            {
                // Fallback: escribir en console
                Console.WriteLine($"[{level}] [{category}] {message}");
            }
        }
    }

    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISimpleLogger _logger;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
            _logger = new SimpleFileLogger("Logs");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var request = context.Request;

            try
            {
                // Loguear request
                var requestLogMessage = $"HTTP {request.Method} {request.Path}{request.QueryString} from {context.Connection.RemoteIpAddress}";
                await _logger.LogAsync("INFORMATION", "HTTP_REQUEST", requestLogMessage);

                // Leer el body si es necesario (para POST/PUT)
                if (request.Method == "POST" || request.Method == "PUT")
                {
                    request.EnableBuffering();
                    using (var reader = new StreamReader(request.Body, leaveOpen: true))
                    {
                        var body = await reader.ReadToEndAsync();
                        request.Body.Position = 0;

                        if (!string.IsNullOrWhiteSpace(body) && body.Length < 500)
                        {
                            await _logger.LogAsync("DEBUG", "HTTP_REQUEST_BODY", $"Request Body: {body}");
                        }
                    }
                }

                // Capturar el response
                var originalBodyStream = context.Response.Body;
                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;

                    await _next(context);

                    stopwatch.Stop();

                    // Loguear response
                    var responseLogMessage = $"HTTP {request.Method} {request.Path} returned {context.Response.StatusCode} in {stopwatch.ElapsedMilliseconds}ms";
                    
                    var logLevel = context.Response.StatusCode >= 500 ? "ERROR" :
                                  context.Response.StatusCode >= 400 ? "WARNING" : "INFORMATION";

                    await _logger.LogAsync(logLevel, "HTTP_RESPONSE", responseLogMessage);

                    // Copiar el response body de vuelta al cliente
                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                await _logger.LogAsync("ERROR", "MIDDLEWARE_ERROR", 
                    $"Unhandled exception in middleware: {ex.Message}", ex);

                throw;
            }
        }
    }

    public static class LoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoggingMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoggingMiddleware>();
        }
    }
}
