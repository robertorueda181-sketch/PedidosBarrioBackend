using MediatR;
using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.Commands.CreateNegocio;
using PedidosBarrio.Application.Commands.DeleteNegocio;
using PedidosBarrio.Application.Commands.UpdateNegocio;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Queries.GetAllNegocios;
using PedidosBarrio.Application.Queries.GetNegocioById;
using PedidosBarrio.Application.Queries.GetNegociosByEmpresa;

namespace PedidosBarrio.Api.EndPoint
{
    public static class NegocioEndpoint
    {
        public static void MapNegocioEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/Negocios")
                           .WithTags("Negocios");

            // GET /api/Negocios
            group.MapGet("/", async (IMediator mediator) =>
            {
                var negocios = await mediator.Send(new GetAllNegociosQuery());
                return Results.Ok(negocios);
            })
            .WithName("GetAllNegocios")
            .WithOpenApi();

            // GET /api/Negocios/{id}
            group.MapGet("/{id:int}", async (int id, IMediator mediator) =>
            {
                var negocio = await mediator.Send(new GetNegocioByIdQuery(id));
                return negocio is not null ? Results.Ok(negocio) : Results.NotFound();
            })
            .WithName("GetNegocioById")
            .WithOpenApi();

            // GET /api/Negocios/empresa/{empresaId}
            group.MapGet("/empresa/{empresaId:Guid}", async (Guid empresaId, IMediator mediator) =>
            {
                var negocios = await mediator.Send(new GetNegociosByEmpresaQuery(empresaId));
                return Results.Ok(negocios);
            })
            .WithName("GetNegociosByEmpresa")
            .WithOpenApi();

            // POST /api/Negocios
            group.MapPost("/", async ([FromBody] CreateNegocioDto createDto, IMediator mediator) =>
            {
                var negocioDto = await mediator.Send(new CreateNegocioCommand(
                    createDto.EmpresaID, createDto.TiposID, createDto.URLNegocio, createDto.Descripcion, createDto.URLOpcional));
                return Results.Created($"/api/Negocios/{negocioDto.NegocioID}", negocioDto);
            })
            .WithName("CreateNegocio")
            .WithOpenApi();

            // PUT /api/Negocios/{id}
            group.MapPut("/{id:int}", async (int id, [FromBody] NegocioDto updateDto, IMediator mediator) =>
            {
                var command = new UpdateNegocioCommand(id, updateDto.EmpresaID, updateDto.TiposID, updateDto.URLNegocio,
                    updateDto.Descripcion);
                await mediator.Send(command);
                return Results.NoContent();
            })
            .WithName("UpdateNegocio")
            .WithOpenApi();

            // DELETE /api/Negocios/{id}
            group.MapDelete("/{id:int}", async (int id, IMediator mediator) =>
            {
                await mediator.Send(new DeleteNegocioCommand(id));
                return Results.NoContent();
            })
            .WithName("DeleteNegocio")
            .WithOpenApi();
        }
    }
}
