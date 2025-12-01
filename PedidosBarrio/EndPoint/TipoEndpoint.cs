using MediatR;
using PedidosBarrio.Application.Queries.GetAllTipos;

namespace PedidosBarrio.Api.EndPoint
{
    public static class TipoEndpoint
    {
        public static void MapTipoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/Tipos")
                           .WithTags("Tipos");

            // GET /api/Tipos
            group.MapGet("/", async (IMediator mediator) =>
            {
                var tipos = await mediator.Send(new GetAllTiposQuery());
                return Results.Ok(tipos);
            })
            .WithName("GetAllTipos")
            .WithOpenApi();
        }
    }
}
