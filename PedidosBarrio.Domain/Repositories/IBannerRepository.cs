using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories
{
    public interface IBannerRepository
    {
        Task<Banner> GetByIdAsync(Guid id);
        Task<IEnumerable<Banner>> GetAllAsync();
        Task<IEnumerable<Banner>> GetByEmpresaIdAsync(Guid empresaId);
        Task<IEnumerable<Banner>> GetActiveByEmpresaIdAsync(Guid empresaId);
        Task<IEnumerable<Banner>> GetAllActiveAsync();
        Task<Guid> AddAsync(Banner banner);
        Task UpdateAsync(Banner banner);
        Task DeleteAsync(Guid id);
    }
}
