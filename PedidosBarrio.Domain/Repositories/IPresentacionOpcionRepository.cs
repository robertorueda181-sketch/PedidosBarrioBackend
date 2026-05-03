using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories
{
    public interface IPresentacionOpcionRepository
    {
        Task<List<PresentacionOpcion>> GetByPresentacionIdAsync(int presentacionId);
        Task<PresentacionOpcion?> GetByIdAsync(int presentacionOpcionId);
        Task<List<PresentacionOpcion>> GetActivasByPresentacionIdAsync(int presentacionId);
        Task AddAsync(PresentacionOpcion entity);
        Task UpdateAsync(PresentacionOpcion entity);
        Task DeleteAsync(int presentacionOpcionId);
    }
}
