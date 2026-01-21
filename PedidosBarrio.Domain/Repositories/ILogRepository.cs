using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories
{
    public interface ILogRepository
    {
        Task<long> AddLogAsync(LogEntry logEntry);
        Task<IEnumerable<LogEntry>> GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate, string level = null);
        Task<IEnumerable<LogEntry>> GetLogsByEmpresaAsync(Guid empresaId, DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<LogEntry>> GetErrorLogsAsync(int limit = 100);
        Task<int> CleanOldLogsAsync(int daysToKeep = 90);
        Task<LogEntry> GetLogByIdAsync(long logId);
        Task<IEnumerable<LogEntry>> GetLogsByCategoryAsync(string category, int limit = 100);
    }
}