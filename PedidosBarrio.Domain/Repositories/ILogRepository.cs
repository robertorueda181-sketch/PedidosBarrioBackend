using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories
{
    public interface ILogRepository
    {
        Task<long> AddLogAsync(Log logEntry);
        Task<IEnumerable<Log>> GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate, string level = null);
        Task<IEnumerable<Log>> GetLogsByEmpresaAsync(Guid empresaId, DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<Log>> GetErrorLogsAsync(int limit = 100);
        Task<int> CleanOldLogsAsync(int daysToKeep = 90);
        Task<Log> GetLogByIdAsync(long logId);
        Task<IEnumerable<Log>> GetLogsByCategoryAsync(string category, int limit = 100);
    }
}