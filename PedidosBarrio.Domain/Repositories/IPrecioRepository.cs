using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories
{
    public interface IPrecioRepository
    {
        Task<Precio?> GetByIdAsync(int id);
        Task<IEnumerable<Precio>> GetByProductoIdAsync(int productoId);
        Task<IEnumerable<Precio>> GetByEmpresaIdAsync(Guid emoresaId);
        Task<Precio?> GetPrecioActualByProductoIdAsync(int productoId);
        Task<int> AddAsync(Precio precio);
        Task UpdateAsync(Precio precio);
        Task DeleteAsync(int id);
    }
}