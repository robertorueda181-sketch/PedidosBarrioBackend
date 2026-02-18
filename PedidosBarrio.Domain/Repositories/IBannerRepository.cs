using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories
{
    public interface IBannerRepository
    {
        Task<Banner> GetByIdAsync(int id);
        Task<IEnumerable<Banner>> GetAllAsync();
        Task<IEnumerable<Banner>> GetByEmpresaIdAsync(Guid empresaId);
        Task<IEnumerable<Banner>> GetActiveByEmpresaIdAsync(Guid empresaId);
        Task<int> AddAsync(Banner banner);
        Task UpdateAsync(Banner banner);
        Task DeleteAsync(int id);
    }
}
