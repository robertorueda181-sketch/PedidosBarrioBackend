using System.Text;

namespace PedidosBarrio.CrossCutting.Logging
{
    public class FileLogger : ILogger
    {
        private readonly string _logDirectory;
        private readonly string _logFileName;
        private readonly object _lockObject = new object();

        public FileLogger(string logDirectory = "Logs")
        {
            _logDirectory = logDirectory;
            _logFileName = Path.Combine(_logDirectory, $"log_{DateTime.UtcNow:yyyy-MM-dd}.txt");

            // Crear directorio si no existe
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        public async Task LogInformationAsync(string message, string category = "General")
        {
            await WriteLogAsync("INFORMATION", category, message);
        }

        public async Task LogWarningAsync(string message, string category = "General")
        {
            await WriteLogAsync("WARNING", category, message);
        }

        public async Task LogErrorAsync(string message, Exception exception = null, string category = "General")
        {
            var logMessage = $"{message}";
            if (exception != null)
            {
                logMessage += $"\n\tException: {exception.Message}\n\tStackTrace: {exception.StackTrace}";
            }
            await WriteLogAsync("ERROR", category, logMessage);
        }

        public async Task LogDebugAsync(string message, string category = "General")
        {
            await WriteLogAsync("DEBUG", category, message);
        }

        private async Task WriteLogAsync(string level, string category, string message)
        {
            try
            {
                var logEntry = new StringBuilder();
                logEntry.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}] [{level}] [{category}]");
                logEntry.AppendLine($"Message: {message}");
                logEntry.AppendLine(new string('-', 80));

                lock (_lockObject)
                {
                    File.AppendAllText(_logFileName, logEntry.ToString(), Encoding.UTF8);
                }

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                // Fallback: escribir en console si falla el archivo
                Console.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }
    }
}
