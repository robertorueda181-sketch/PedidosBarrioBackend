using MediatR;
using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.Commands.CreateNegocio;
using PedidosBarrio.Application.Commands.DeleteNegocio;
using PedidosBarrio.Application.Commands.UpdateNegocio;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Queries.GetAllNegocios;
using PedidosBarrio.Application.Queries.GetNegocioById;
using PedidosBarrio.Application.Queries.GetNegociosByEmpresa;
using PedidosBarrio.Application.Queries.GetNegocioByCodigoEmpresa;

namespace PedidosBarrio.Api.EndPoint
{
    public static class NegocioEndpoint
    {
        public static void MapNegocioEndpoints(this IEndpointRouteBuilder app)
        {
            // Grupo PÚBLICO sin autenticación
            var publicGroup = app.MapGroup("/api/Negocios")
                           .WithTags("Negocios");

            // GET /api/Negocios/publico/detalle/{codigoEmpresa} - 🌍 PÚBLICO - Obtener detalle completo del negocio
            publicGroup.MapGet("/publico/detalle/{codigoEmpresa}", async (string codigoEmpresa, IMediator mediator) =>
            {
                try
                {
                    var negocio = await mediator.Send(new GetNegocioByCodigoEmpresaQuery(codigoEmpresa));
                    return negocio is not null ? Results.Ok(negocio) : Results.NotFound(new { message = "Negocio no encontrado" });
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { success = false, message = ex.Message });
                }
            })
            .WithName("GetNegocioDetallePublico")
            .WithOpenApi()
            .WithSummary("🌍 Obtener detalle completo del negocio")
            .WithDescription("Obtiene la información completa del negocio incluyendo: categorías, productos, redes sociales, dirección, teléfono, etc. No requiere autenticación.");

            // Grupo privado con autenticación requerida
            var group = app.MapGroup("/api/Negocios")
                           .WithTags("Negocios")
                           .RequireAuthorization();

            // GET /api/Negocios
            group.MapGet("/", async (IMediator mediator) =>
            {
                var negocios = await mediator.Send(new GetAllNegociosQuery());
                return Results.Ok(negocios);
            })
            .WithName("GetAllNegocios")
            .WithOpenApi();

            // GET /api/Negocios/codigo/{codigoEmpresa} - ? NUEVO
            group.MapGet("/codigo/{codigoEmpresa}", async (string codigoEmpresa, IMediator mediator) =>
            {
                var negocio = await mediator.Send(new GetNegocioByCodigoEmpresaQuery(codigoEmpresa));
                return negocio is not null ? Results.Ok(negocio) : Results.NotFound();
            })
            .WithName("GetNegocioByCodigoEmpresa")
            .WithOpenApi();

            // GET /api/Negocios/{id}
            group.MapGet("/{id}", async (string id, IMediator mediator) =>
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

        }
    }
}

