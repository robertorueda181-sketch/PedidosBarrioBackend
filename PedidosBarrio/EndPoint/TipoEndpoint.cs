using MediatR;
using PedidosBarrio.Application.Queries.GetAllTipos;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Api.EndPoint
{
    public static class TipoEndpoint
    {
        public static void MapTipoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/Tipos")
                           .WithTags("Tipos");

            // GET /api/Tipos
            group.MapGet("/", GetAllTipos)
                .WithName("GetAllTipos")
                .WithOpenApi()
                .Produces<IEnumerable<TipoDto>>(StatusCodes.Status200OK)
                .WithDescription("Obtiene todos los tipos, opcionalmente filtrados por tipo o parámetro")
                .WithSummary("Obtener Tipos");

            // GET /api/Tipos/valores - Obtener valores distintos de la columna Tipo (tipo_ser, tipo_neg)
            group.MapGet("/valores", async (IMediator mediator) =>
            {
                var tipos = await mediator.Send(new PedidosBarrio.Application.Queries.GetTipos.GetTiposQuery());
                return Results.Ok(tipos);
            })
            .WithName("GetTiposValores")
            .WithOpenApi()
            .Produces<IEnumerable<TipoDto>>(StatusCodes.Status200OK)
            .WithDescription("Obtiene los valores distintos de la columna Tipo filtrando por parametros tipo_ser y tipo_neg")
            .WithSummary("Obtener valores de Tipo");
        }

        private static async Task<IResult> GetAllTipos(
            IMediator mediator,
            [Microsoft.AspNetCore.Mvc.FromQuery] string? tipo = null,
            [Microsoft.AspNetCore.Mvc.FromQuery] string? param = null)
        {
            var query = new GetAllTiposQuery
            {
                Tipo = tipo,
                Param = param
            };
            var tipos = await mediator.Send(query);
            return Results.Ok(tipos);
        }
    }
}

