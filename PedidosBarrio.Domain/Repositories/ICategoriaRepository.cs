using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories
{
    public interface ICategoriaRepository
    {
        Task<Categoria> GetByIdAsync(short categoriaId);
        Task<IEnumerable<Categoria>> GetAllAsync(Guid empresaId);
        Task<IEnumerable<Categoria>> GetActiveAsync();
        Task<IEnumerable<Categoria>> GetByEmpresaIdAsync(Guid empresaId);
        Task<IEnumerable<Categoria>> GetByEmpresaIdMostrandoAsync(Guid empresaId);
        Task<short> AddAsync(Categoria categoria);  
        Task UpdateAsync(Categoria categoria);
        Task SoftDeleteAsync(short categoriaId); // Soft delete (sets Activo = false)
    }
}
