using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories
{
    public interface ISearchRepository
    {
        Task<IEnumerable<SearchResult>> SearchAsync(string term);
    }
}
