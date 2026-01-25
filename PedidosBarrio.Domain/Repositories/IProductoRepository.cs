using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories
{
    public interface IProductoRepository
    {
        Task<Producto> GetByIdAsync(int id, Guid empresaId);
        Task<IEnumerable<Producto>> GetByEmpresaIdAsync(Guid empresaId);
        Task<int> AddAsync(Producto producto);
        Task UpdateAsync(Producto producto);
        Task DeleteAsync(int id);
    }
}
