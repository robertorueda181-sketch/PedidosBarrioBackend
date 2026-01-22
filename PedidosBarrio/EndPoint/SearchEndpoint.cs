using MediatR;
using PedidosBarrio.Application.Queries.Search;

namespace PedidosBarrio.Api.EndPoint
{
    public static class SearchEndpoint
    {
        public static void MapSearchEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/Search").WithTags("Search");

            group.MapGet("/", async (string? q, IMediator mediator) =>
            {
                var term = q ?? string.Empty;
                var results = await mediator.Send(new SearchQuery(term));
                return Results.Ok(results);
            })
            .WithName("Search")
            .WithOpenApi();
        }
    }
}
