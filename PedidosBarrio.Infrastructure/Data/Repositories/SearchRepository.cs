using Dapper;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Common;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class SearchRepository : GenericRepository, ISearchRepository
    {
        public SearchRepository(IDbConnectionProvider connectionProvider) : base(connectionProvider)
        {
        }

        public async Task<IEnumerable<SearchResult>> SearchAsync(string term)
        {
            using var connection = CreateConnection();
            if (string.IsNullOrWhiteSpace(term))
                return Enumerable.Empty<SearchResult>();

            var results = await connection.QueryAsync<SearchResult>(
                "SELECT * FROM public.fn_search(@p_term)", new { p_term = term });

            return results;
        }
    }
}
