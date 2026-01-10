using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.Search
{
    public class SearchQuery : IRequest<IEnumerable<SearchResultDto>>
    {
        public string Term { get; set; }

        public SearchQuery(string term)
        {
            Term = term;
        }
    }
}
