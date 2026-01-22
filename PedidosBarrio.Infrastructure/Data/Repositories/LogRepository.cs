using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;
using PedidosBarrio.Infrastructure.Data.Repositories.Base;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class LogRepository : EfCoreRepository<Log>, ILogRepository
    {
        public LogRepository(PedidosBarrioDbContext context) : base(context)
        {
        }

        public async Task<long> AddLogAsync(Log log)
        {
            await base.AddAsync(log);
            return log.LogID;
        }

        public async Task<IEnumerable<Log>> GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate, string level = null)
        {
            var query = _context.Logs.AsQueryable()
               .Where(l => l.Timestamp >= startDate && l.Timestamp <= endDate);
            
            if (!string.IsNullOrEmpty(level))
            {
                query = query.Where(l => l.Level == level);
            }

            return await query.OrderByDescending(l => l.Timestamp).ToListAsync();
        }

        public async Task<IEnumerable<Log>> GetLogsByEmpresaAsync(Guid empresaId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Logs.AsQueryable()
               .Where(l => l.EmpresaID == empresaId);

            if (startDate.HasValue) query = query.Where(l => l.Timestamp >= startDate.Value);
            if (endDate.HasValue) query = query.Where(l => l.Timestamp <= endDate.Value);

            return await query.OrderByDescending(l => l.Timestamp).ToListAsync();
        }

        public async Task<IEnumerable<Log>> GetErrorLogsAsync(int limit = 100)
        {
            return await _context.Logs
               .Where(l => l.Level == "Error" || l.Level == "Fatal" || l.Level == "Critical")
               .OrderByDescending(l => l.Timestamp)
               .Take(limit)
               .ToListAsync();
        }

        public async Task<int> CleanOldLogsAsync(int daysToKeep = 90)
        {
            var dateThreshold = DateTime.UtcNow.AddDays(-daysToKeep);
            var logsToDelete = _context.Logs.Where(l => l.Timestamp < dateThreshold);
            _context.Logs.RemoveRange(logsToDelete);
            return await _context.SaveChangesAsync();
        }

        public async Task<Log> GetLogByIdAsync(long logId)
        {
            return await GetByIdAsync(logId);
        }

        public async Task<IEnumerable<Log>> GetLogsByCategoryAsync(string category, int limit = 100)
        {
             return await _context.Logs
               .Where(l => l.Category == category)
               .OrderByDescending(l => l.Timestamp)
               .Take(limit)
               .ToListAsync();
        }
    }
}