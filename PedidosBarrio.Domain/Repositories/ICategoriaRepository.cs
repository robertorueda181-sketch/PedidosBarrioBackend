using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories
{
    public interface ICategoriaRepository
    {
        Task<Categoria> GetByIdAsync(short categoriaId);
        Task<IEnumerable<Categoria>> GetAllAsync();
        Task<IEnumerable<Categoria>> GetByEmpresaIdAsync(Guid empresaId);
        Task<IEnumerable<Categoria>> GetByEmpresaIdMostrandoAsync(Guid empresaId);
        Task<int> AddAsync(Categoria categoria);
        Task UpdateAsync(Categoria categoria);
        Task DeleteAsync(short categoriaId);
    }
}
