using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories
{
    public interface IInmuebleRepository
    {
        Task<Inmueble> GetByIdAsync(int id);
        Task<IEnumerable<Inmueble>> GetAllAsync();
        Task<IEnumerable<Inmueble>> GetByEmpresaIdAsync(int empresaId);
        Task<int> AddAsync(Inmueble inmueble);
        Task UpdateAsync(Inmueble inmueble);
        Task DeleteAsync(int id);
    }
}
