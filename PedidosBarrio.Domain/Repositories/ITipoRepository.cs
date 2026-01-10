using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories
{
    public interface ITipoRepository
    {
        Task<IEnumerable<Tipo>> GetByCategoriaAsync(string categoria, string param);
        Task<IEnumerable<Tipo>> GetTiposPorParametroAsync();
    }
}
