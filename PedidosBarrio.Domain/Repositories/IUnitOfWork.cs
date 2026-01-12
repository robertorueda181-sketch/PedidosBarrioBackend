using System;
using System.Data;
using System.Threading.Tasks;

namespace PedidosBarrio.Domain.Repositories
{
    /// <summary>
    /// Patrón Unit of Work para manejar transacciones
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Ejecuta una operación dentro de una transacción
        /// </summary>
        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        /// <summary>
        /// Ejecuta una operación sin retorno dentro de una transacción
        /// </summary>
        Task ExecuteInTransactionAsync(Func<Task> operation, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    }
}
