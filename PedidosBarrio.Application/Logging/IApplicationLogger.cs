namespace PedidosBarrio.Application.Logging
{
    /// <summary>
    /// Logger simple para la capa de aplicación (handlers, validators, etc.)
    /// No depende de infraestructura
    /// </summary>
    public interface IApplicationLogger
    {
        Task LogInformationAsync(string message, string category = "Application");
        Task LogWarningAsync(string message, string category = "Application");
        Task LogErrorAsync(string message, Exception exception = null, string category = "Application");
        Task LogDebugAsync(string message, string category = "Application");
    }

    /// <summary>
    /// Implementación simple de logger que escribe a console y archivo
    /// </summary>
    public class ConsoleFileLogger : IApplicationLogger
    {
        private readonly string _logDirectory;
        private readonly object _lockObject = new object();

        public ConsoleFileLogger(string logDirectory = "Logs")
        {
            _logDirectory = logDirectory;
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        public async Task LogInformationAsync(string message, string category = "Application")
        {
            await LogAsync("INFORMATION", category, message);
        }

        public async Task LogWarningAsync(string message, string category = "Application")
        {
            await LogAsync("WARNING", category, message);
        }

        public async Task LogErrorAsync(string message, Exception exception = null, string category = "Application")
        {
            var fullMessage = exception != null 
                ? $"{message}\nException: {exception.Message}\nStackTrace: {exception.StackTrace}"
                : message;
            
            await LogAsync("ERROR", category, fullMessage);
        }

        public async Task LogDebugAsync(string message, string category = "Application")
        {
            await LogAsync("DEBUG", category, message);
        }

        private async Task LogAsync(string level, string category, string message)
        {
            try
            {
                var logFileName = Path.Combine(_logDirectory, $"log_{DateTime.UtcNow:yyyy-MM-dd}.txt");
                var logEntry = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}] [{level}] [{category}]\n{message}\n{new string('-', 80)}\n";

                // Escribir en archivo de forma thread-safe
                lock (_lockObject)
                {
                    File.AppendAllText(logFileName, logEntry, System.Text.Encoding.UTF8);
                }

                // También escribir en console para desarrollo
                ConsoleColor originalColor = Console.ForegroundColor;
                try
                {
                    Console.ForegroundColor = level switch
                    {
                        "ERROR" => ConsoleColor.Red,
                        "WARNING" => ConsoleColor.Yellow,
                        "INFORMATION" => ConsoleColor.Green,
                        "DEBUG" => ConsoleColor.Cyan,
                        _ => ConsoleColor.White
                    };

                    Console.WriteLine($"[{level}] [{category}] {message.Split('\n')[0]}");
                }
                finally
                {
                    Console.ForegroundColor = originalColor;
                }

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                // Fallback: escribir en console si falla el archivo
                Console.WriteLine($"Error al escribir log: {ex.Message}");
            }
        }
    }
}
