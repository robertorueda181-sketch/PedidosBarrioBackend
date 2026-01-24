using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Implementación del patrón Unit of Work para transacciones usando Entity Framework Core
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PedidosBarrioDbContext _context;

        public UnitOfWork(PedidosBarrioDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<T> ExecuteInTransactionAsync<T>(
            Func<Task<T>> operation,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (_context.Database.CurrentTransaction != null)
            {
                return await operation();
            }

            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync(isolationLevel);
                try
                {
                    var result = await operation();
                    await transaction.CommitAsync();
                    return result;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public async Task ExecuteInTransactionAsync(
            Func<Task> operation,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (_context.Database.CurrentTransaction != null)
            {
                await operation();
                return;
            }

            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync(isolationLevel);
                try
                {
                    await operation();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }
    }
}
