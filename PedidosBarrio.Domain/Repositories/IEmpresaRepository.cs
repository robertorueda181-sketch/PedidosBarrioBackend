using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories
{
    public interface IEmpresaRepository
    {
        Task<Empresa> GetByIdAsync(Guid id);
        Task<Empresa> GetByEmailAsync(string email);
        Task<IEnumerable<Empresa>> GetAllAsync();
        Task AddAsync(Empresa empresa);
        Task DeleteAsync(Guid id);
    }
}
