using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.Search
{
    public class SearchQueryHandler : IRequestHandler<SearchQuery, IEnumerable<SearchResultDto>>
    {
        private readonly ISearchRepository _searchRepository;

        public SearchQueryHandler(ISearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }

        public async Task<IEnumerable<SearchResultDto>> Handle(SearchQuery request, CancellationToken cancellationToken)
        {
            var term = request.Term?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(term))
                return Enumerable.Empty<SearchResultDto>();

            var results = await _searchRepository.SearchAsync(term);
            return results.Select(r => new SearchResultDto
            {
                Type = r.Type,
                Id = r.Id,
                Title = r.Title,
                Description = r.Description,
                ImageUrl = r.ImageUrl,
                Location = r.Location,
                Category = r.Category,
                Url = r.Url,
                Price = r.Price,
                Operacion = r.Operacion,
                Medidas = r.Medidas,
                Dormitorios = r.Dormitorios,
                Banos = r.Banos
            }).ToList();
        }
    }
}
