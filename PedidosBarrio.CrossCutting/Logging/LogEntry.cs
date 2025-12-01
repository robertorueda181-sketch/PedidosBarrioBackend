namespace PedidosBarrio.CrossCutting.Logging
{
    public class LogEntry
    {
        public int LogID { get; set; }
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } // Information, Warning, Error, Debug
        public string Category { get; set; }
        public string Message { get; set; }
        public string ExceptionMessage { get; set; }
        public string StackTrace { get; set; }
        public string Source { get; set; } // IP, User, etc.

        public LogEntry() { }

        public LogEntry(string level, string category, string message, Exception exception = null, string source = null)
        {
            Timestamp = DateTime.UtcNow;
            Level = level;
            Category = category;
            Message = message;
            ExceptionMessage = exception?.Message;
            StackTrace = exception?.StackTrace;
            Source = source;
        }
    }
}
