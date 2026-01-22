namespace PedidosBarrio.Domain.Entities
{
    public class LogEntry
    {
        public long LogID { get; set; }
        public DateTime Timestamp { get; set; }
        public string Level { get; set; }
        public string Logger { get; set; }
        public string Message { get; set; }
        public string? Exception { get; set; }
        public string? Properties { get; set; } // JSON serializado
        public string? MachineName { get; set; }
        public int? ProcessId { get; set; }
        public int? ThreadId { get; set; }
        public string? UserId { get; set; }
        public string? RequestId { get; set; }
        public string? RequestPath { get; set; }
        public string? HttpMethod { get; set; }
        public int? StatusCode { get; set; }
        public int? Duration { get; set; }
        public Guid? EmpresaID { get; set; }
        public string Category { get; set; } = "General";

        public LogEntry(string level, string logger, string message, string category = "General")
        {
            Level = level;
            Logger = logger;
            Message = message;
            Category = category;
            Timestamp = DateTime.UtcNow;
        }

        private LogEntry() { }
    }
}
