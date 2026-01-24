using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories
{
    public interface ISuscripcionRepository
    {
        Task<Suscripcion> GetByIdAsync(int id);
        Task<IEnumerable<Suscripcion>> GetAllAsync();
        Task<IEnumerable<Suscripcion>> GetByEmpresaIdAsync(Guid empresaId);
        Task<int> AddAsync(Suscripcion suscripcion);
        Task UpdateAsync(Suscripcion suscripcion);
        Task DeleteAsync(int id);
    }
}
