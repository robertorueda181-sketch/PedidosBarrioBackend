using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories
{
    public interface IPresentacionRepository
    {
        Task<Presentacion?> GetByIdAsync(int id);
        Task<IEnumerable<Presentacion>> GetByProductoIdAsync(int productoId);
        Task<IEnumerable<Presentacion>> GetByProductoIdWithOpcionesAsync(int productoId);
        Task<int> AddAsync(Presentacion presentacion);
        Task UpdateAsync(Presentacion presentacion);
        Task DeleteAsync(int id);
    }
}
