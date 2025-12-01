using MediatR;
using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.Commands.CreateInmueble;
using PedidosBarrio.Application.Commands.DeleteInmueble;
using PedidosBarrio.Application.Commands.UpdateInmueble;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Queries.GetAllInmuebles;
using PedidosBarrio.Application.Queries.GetInmuebleById;
using PedidosBarrio.Application.Queries.GetInmueblesByEmpresa;

namespace PedidosBarrio.Api.EndPoint
{
    public static class InmuebleEndpoint
    {
        public static void MapInmuebleEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/Inmuebles")
                           .WithTags("Inmuebles");

            // GET /api/Inmuebles
            group.MapGet("/", async (IMediator mediator) =>
            {
                var inmuebles = await mediator.Send(new GetAllInmueblesQuery());
                return Results.Ok(inmuebles);
            })
            .WithName("GetAllInmuebles")
            .WithOpenApi();

            // GET /api/Inmuebles/{id}
            group.MapGet("/{id:int}", async (int id, IMediator mediator) =>
            {
                var inmueble = await mediator.Send(new GetInmuebleByIdQuery(id));
                return inmueble is not null ? Results.Ok(inmueble) : Results.NotFound();
            })
            .WithName("GetInmuebleById")
            .WithOpenApi();

            // GET /api/Inmuebles/empresa/{empresaId}
            group.MapGet("/empresa/{empresaId:int}", async (int empresaId, IMediator mediator) =>
            {
                var inmuebles = await mediator.Send(new GetInmueblesByEmpresaQuery(empresaId));
                return Results.Ok(inmuebles);
            })
            .WithName("GetInmueblesByEmpresa")
            .WithOpenApi();

            // POST /api/Inmuebles
            group.MapPost("/", async ([FromBody] CreateInmuebleDto createDto, IMediator mediator) =>
            {
                var inmuebleDto = await mediator.Send(new CreateInmuebleCommand(
                    createDto.EmpresaID, createDto.TiposID, createDto.Precio, createDto.Medidas,
                    createDto.Ubicacion, createDto.Dormitorios, createDto.Banos, createDto.Descripcion));
                return Results.Created($"/api/Inmuebles/{inmuebleDto.InmuebleID}", inmuebleDto);
            })
            .WithName("CreateInmueble")
            .WithOpenApi();

            // PUT /api/Inmuebles/{id}
            group.MapPut("/{id:int}", async (int id, [FromBody] InmuebleDto updateDto, IMediator mediator) =>
            {
                var command = new UpdateInmuebleCommand(id, updateDto.EmpresaID, updateDto.TiposID, updateDto.Precio,
                    updateDto.Medidas, updateDto.Ubicacion, updateDto.Dormitorios, updateDto.Banos, updateDto.Descripcion);
                await mediator.Send(command);
                return Results.NoContent();
            })
            .WithName("UpdateInmueble")
            .WithOpenApi();

            // DELETE /api/Inmuebles/{id}
            group.MapDelete("/{id:int}", async (int id, IMediator mediator) =>
            {
                await mediator.Send(new DeleteInmuebleCommand(id));
                return Results.NoContent();
            })
            .WithName("DeleteInmueble")
            .WithOpenApi();
        }
    }
}
