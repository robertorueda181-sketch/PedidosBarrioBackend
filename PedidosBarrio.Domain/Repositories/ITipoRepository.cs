using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories
{
    public interface ITipoRepository
    {
        Task<IEnumerable<Tipo>> GetByCategoriaAsync(string categoria);
        Task<IEnumerable<Tipo>> GetAllAsync();
    }
}
