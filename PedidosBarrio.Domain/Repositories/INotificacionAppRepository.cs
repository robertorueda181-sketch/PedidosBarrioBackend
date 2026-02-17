using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories
{
    public interface INotificacionAppRepository
    {
        Task<int> AddAsync(NotificacionApp notificacion);
    }
}
