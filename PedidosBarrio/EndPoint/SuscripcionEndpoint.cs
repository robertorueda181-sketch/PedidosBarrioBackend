using MediatR;
using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.Commands.CreateSuscripcion;
using PedidosBarrio.Application.Commands.DeleteSuscripcion;
using PedidosBarrio.Application.Commands.UpdateSuscripcion;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Queries.GetAllSuscripciones;
using PedidosBarrio.Application.Queries.GetSuscripcionById;
using PedidosBarrio.Application.Queries.GetSuscripcionesByEmpresa;

namespace PedidosBarrio.Api.EndPoint
{
    public static class SuscripcionEndpoint
    {
        public static void MapSuscripcionEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/Suscripciones")
                           .WithTags("Suscripciones");

            // GET /api/Suscripciones
            group.MapGet("/", async (IMediator mediator) =>
            {
                var suscripciones = await mediator.Send(new GetAllSuscripcionesQuery());
                return Results.Ok(suscripciones);
            })
            .WithName("GetAllSuscripciones")
            .WithOpenApi();

            // GET /api/Suscripciones/{id}
            group.MapGet("/{id:int}", async (int id, IMediator mediator) =>
            {
                var suscripcion = await mediator.Send(new GetSuscripcionByIdQuery(id));
                return suscripcion is not null ? Results.Ok(suscripcion) : Results.NotFound();
            })
            .WithName("GetSuscripcionById")
            .WithOpenApi();

            // GET /api/Suscripciones/empresa/{empresaId}
            group.MapGet("/empresa/{empresaId:int}", async (int empresaId, IMediator mediator) =>
            {
                var suscripciones = await mediator.Send(new GetSuscripcionesByEmpresaQuery(empresaId));
                return Results.Ok(suscripciones);
            })
            .WithName("GetSuscripcionesByEmpresa")
            .WithOpenApi();

            // POST /api/Suscripciones
            group.MapPost("/", async ([FromBody] CreateSuscripcionDto createDto, IMediator mediator) =>
            {
                var suscripcionDto = await mediator.Send(new CreateSuscripcionCommand(createDto.EmpresaID, createDto.Monto, createDto.FechaFin));
                return Results.Created($"/api/Suscripciones/{suscripcionDto.SuscripcionID}", suscripcionDto);
            })
            .WithName("CreateSuscripcion")
            .WithOpenApi();

            // PUT /api/Suscripciones/{id}
            group.MapPut("/{id:int}", async (int id, [FromBody] SuscripcionDto updateDto, IMediator mediator) =>
            {
                var command = new UpdateSuscripcionCommand(id, updateDto.EmpresaID, updateDto.Monto, updateDto.FechaFin, updateDto.Activa);
                await mediator.Send(command);
                return Results.NoContent();
            })
            .WithName("UpdateSuscripcion")
            .WithOpenApi();

            // DELETE /api/Suscripciones/{id}
            group.MapDelete("/{id:int}", async (int id, IMediator mediator) =>
            {
                await mediator.Send(new DeleteSuscripcionCommand(id));
                return Results.NoContent();
            })
            .WithName("DeleteSuscripcion")
            .WithOpenApi();
        }
    }
}
