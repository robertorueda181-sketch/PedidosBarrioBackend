using System;
using System.Data;
using System.Threading.Tasks;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Common;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Implementación del patrón Unit of Work para transacciones
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbConnectionProvider _connectionProvider;

        public UnitOfWork(IDbConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<T> ExecuteInTransactionAsync<T>(
            Func<Task<T>> operation,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            using (var connection = _connectionProvider.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(isolationLevel))
                {
                    try
                    {
                        var result = await operation();
                        transaction.Commit();
                        return result;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public async Task ExecuteInTransactionAsync(
            Func<Task> operation,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            using (var connection = _connectionProvider.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(isolationLevel))
                {
                    try
                    {
                        await operation();
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }
    }
}
