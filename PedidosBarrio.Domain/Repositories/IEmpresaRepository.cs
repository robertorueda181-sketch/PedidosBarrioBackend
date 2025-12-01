using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories
{
    public interface IEmpresaRepository
    {
        Task<Empresa> GetByIdAsync(Guid id);
        Task<IEnumerable<Empresa>> GetAllAsync();
        Task AddAsync(Empresa empresa);
        Task UpdateAsync(Empresa empresa);
        Task DeleteAsync(Guid id);
    }
}
