using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories
{
    public interface ITipoRepository
    {
        Task<IEnumerable<Tipo>> GetByCategoriaAsync(string param);
        Task<IEnumerable<Tipo>> GetTiposPorParametroAsync();
    }
}
