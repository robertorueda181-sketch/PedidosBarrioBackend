using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories
{
    public interface IIaModeracionLogRepository
    {
        Task<IaModeracionLog> AddAsync(IaModeracionLog log);
    }
}
