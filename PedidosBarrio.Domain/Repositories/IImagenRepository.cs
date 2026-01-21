using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories
{
    public interface IImagenRepository
    {
        Task<Imagen> GetByIdAsync(int id);
        Task<IEnumerable<Imagen>> GetAllAsync();
        Task<IEnumerable<Imagen>> GetByProductoIdAsync(int productoId, string tipo = "PRODUCT");
        Task<IEnumerable<Imagen>> GetByEmpresaIdAsync(Guid empresaId);
        Task<Imagen> GetPrincipalByProductoIdAsync(int productoId);
        Task<int> AddAsync(Imagen imagen);
        Task UpdateAsync(Imagen imagen);
        Task DeleteAsync(int id);
        Task SetPrincipalAsync(int imagenId, int productoId, Guid empresaId);
    }
}
