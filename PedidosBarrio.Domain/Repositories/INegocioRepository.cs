using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories
{
    public interface INegocioRepository
    {
        Task<Negocio> GetByIdAsync(int id);
        Task<IEnumerable<Negocio>> GetAllAsync();
        Task<IEnumerable<Negocio>> GetByEmpresaIdAsync(Guid empresaId);
        Task<int> AddAsync(Negocio negocio);
        Task UpdateAsync(Negocio negocio);
        Task DeleteAsync(int id);
    }
}
