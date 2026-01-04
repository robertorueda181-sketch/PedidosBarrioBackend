using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories
{
    public interface IImagenRepository
    {
        Task<Imagen> GetByIdAsync(int id);
        Task<IEnumerable<Imagen>> GetAllAsync();
        Task<IEnumerable<Imagen>> GetByProductoIdAsync(int productoId, string tipo);
        Task<int> AddAsync(Imagen imagen);
        Task UpdateAsync(Imagen imagen);
        Task DeleteAsync(int id);
    }
}
