using Dapper;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Common;
using System.Data;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly IDbConnectionProvider _dbConnectionProvider;

        public LogRepository(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }

        public async Task<long> AddLogAsync(LogEntry logEntry)
        {
            using var connection = _dbConnectionProvider.CreateConnection();

            const string sql = @"
                INSERT INTO ""Logs"" (
                    ""Timestamp"", ""Level"", ""Logger"", ""Message"", ""Exception"", 
                    ""Properties"", ""MachineName"", ""ProcessId"", ""ThreadId"", 
                    ""UserId"", ""RequestId"", ""RequestPath"", ""HttpMethod"", 
                    ""StatusCode"", ""Duration"", ""EmpresaID"", ""Category""
                ) VALUES (
                    @Timestamp, @Level, @Logger, @Message, @Exception,
                    @Properties, @MachineName, @ProcessId, @ThreadId,
                    @UserId, @RequestId, @RequestPath, @HttpMethod,
                    @StatusCode, @Duration, @EmpresaID, @Category
                ) RETURNING ""LogID""";

            var logId = await connection.QuerySingleAsync<long>(sql, logEntry);
            return logId;
        }

        public async Task<IEnumerable<LogEntry>> GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate, string level = null)
        {
            using var connection = _dbConnectionProvider.CreateConnection();

            var sql = @"
                SELECT * FROM ""Logs""
                WHERE ""Timestamp"" >= @StartDate AND ""Timestamp"" <= @EndDate";

            var parameters = new DynamicParameters();
            parameters.Add("@StartDate", startDate);
            parameters.Add("@EndDate", endDate);

            if (!string.IsNullOrEmpty(level))
            {
                sql += @" AND ""Level"" = @Level";
                parameters.Add("@Level", level);
            }

            sql += @" ORDER BY ""Timestamp"" DESC LIMIT 1000";

            return await connection.QueryAsync<LogEntry>(sql, parameters);
        }

        public async Task<IEnumerable<LogEntry>> GetLogsByEmpresaAsync(Guid empresaId, DateTime? startDate = null, DateTime? endDate = null)
        {
            using var connection = _dbConnectionProvider.CreateConnection();

            var sql = @"SELECT * FROM ""Logs"" WHERE ""EmpresaID"" = @EmpresaId";
            var parameters = new DynamicParameters();
            parameters.Add("@EmpresaId", empresaId);

            if (startDate.HasValue)
            {
                sql += @" AND ""Timestamp"" >= @StartDate";
                parameters.Add("@StartDate", startDate.Value);
            }

            if (endDate.HasValue)
            {
                sql += @" AND ""Timestamp"" <= @EndDate";
                parameters.Add("@EndDate", endDate.Value);
            }

            sql += @" ORDER BY ""Timestamp"" DESC LIMIT 500";

            return await connection.QueryAsync<LogEntry>(sql, parameters);
        }

        public async Task<IEnumerable<LogEntry>> GetErrorLogsAsync(int limit = 100)
        {
            using var connection = _dbConnectionProvider.CreateConnection();

            const string sql = @"
                SELECT * FROM ""Logs""
                WHERE ""Level"" IN ('Error', 'Fatal', 'Critical')
                ORDER BY ""Timestamp"" DESC
                LIMIT @Limit";

            return await connection.QueryAsync<LogEntry>(sql, new { Limit = limit });
        }

        public async Task<int> CleanOldLogsAsync(int daysToKeep = 90)
        {
            using var connection = _dbConnectionProvider.CreateConnection();

            const string sql = @"SELECT sp_CleanOldLogs(@DaysToKeep)";

            var deletedCount = await connection.QuerySingleAsync<int>(sql, new { DaysToKeep = daysToKeep });
            return deletedCount;
        }

        public async Task<LogEntry> GetLogByIdAsync(long logId)
        {
            using var connection = _dbConnectionProvider.CreateConnection();

            const string sql = @"SELECT * FROM ""Logs"" WHERE ""LogID"" = @LogId";

            return await connection.QueryFirstOrDefaultAsync<LogEntry>(sql, new { LogId = logId });
        }

        public async Task<IEnumerable<LogEntry>> GetLogsByCategoryAsync(string category, int limit = 100)
        {
            using var connection = _dbConnectionProvider.CreateConnection();

            const string sql = @"
                SELECT * FROM ""Logs""
                WHERE ""Category"" = @Category
                ORDER BY ""Timestamp"" DESC
                LIMIT @Limit";

            return await connection.QueryAsync<LogEntry>(sql, new { Category = category, Limit = limit });
        }
    }
}