using MediatR;
using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.Commands.CreateNegocio;
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
            // Grupo PÚBLICO sin autenticación (Lectura)
            var publicGroup = app.MapGroup("/api/Negocios")
                           .WithTags("Negocios");

            // GET /api/Negocios - 🌍 PÚBLICO - Listar todos los negocios
            publicGroup.MapGet("/", async (IMediator mediator) =>
            {
                var negocios = await mediator.Send(new GetAllNegociosQuery());
                return Results.Ok(negocios);
            })
            .WithName("GetAllNegocios")
            .WithOpenApi()
            .WithSummary("🌍 Listar todos los negocios")
            .WithDescription("Obtiene la lista de todos los negocios. No requiere autenticación.");

            // GET /api/Negocios/{id} - 🌍 PÚBLICO - Obtener negocio por ID
            publicGroup.MapGet("/{id}", async (string id, IMediator mediator) =>
            {
                var negocio = await mediator.Send(new GetNegocioByIdQuery(id));
                return negocio is not null ? Results.Ok(negocio) : Results.NotFound(new { message = "Negocio no encontrado" });
            })
            .WithName("GetNegocioById")
            .WithOpenApi()
            .WithSummary("🌍 Obtener negocio por ID")
            .WithDescription("Obtiene los detalles de un negocio específico por su ID. No requiere autenticación.");

            // GET /api/Negocios/codigo/{codigoEmpresa} - 🌍 PÚBLICO - Obtener detalle completo del negocio
            publicGroup.MapGet("/codigo/{codigoEmpresa}", async (string codigoEmpresa, IMediator mediator) =>
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
            .WithName("GetNegocioByCodigoEmpresa")
            .WithOpenApi()
            .WithSummary("🌍 Obtener negocio por código de empresa")
            .WithDescription("Obtiene la información completa del negocio incluyendo: categorías, productos, redes sociales, dirección, teléfono, etc. No requiere autenticación.");

            // GET /api/Negocios/publico/detalle/{codigoEmpresa} - 🌍 PÚBLICO - Obtener detalle completo del negocio (Ruta alternativa)
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
            .WithSummary("🌍 Obtener detalle completo del negocio (ruta alternativa)")
            .WithDescription("Obtiene la información completa del negocio. No requiere autenticación. (Ruta alternativa a /codigo/{codigoEmpresa})");

            // Grupo privado con autenticación requerida (Escritura/Modificación)
            var protectedGroup = app.MapGroup("/api/Negocios")
                           .WithTags("Negocios")
                           .RequireAuthorization();

            // GET /api/Negocios/empresa/{empresaId} - 🔐 PROTEGIDO - Obtener negocios por empresa
            protectedGroup.MapGet("/empresa/{empresaId:Guid}", async (Guid empresaId, IMediator mediator) =>
            {
                var negocios = await mediator.Send(new GetNegociosByEmpresaQuery(empresaId));
                return Results.Ok(negocios);
            })
            .WithName("GetNegociosByEmpresa")
            .WithOpenApi()
            .WithSummary("🔐 Obtener negocios por empresa")
            .WithDescription("Obtiene los negocios asociados a una empresa específica. Requiere autenticación.");

            // POST /api/Negocios - 🔐 PROTEGIDO - Crear negocio
            protectedGroup.MapPost("/", async ([FromBody] CreateNegocioDto createDto, IMediator mediator) =>
            {
                var negocioDto = await mediator.Send(new CreateNegocioCommand(
                    createDto.EmpresaID, createDto.TiposID, createDto.URLNegocio, createDto.Descripcion, createDto.URLOpcional));
                return Results.Created($"/api/Negocios/{negocioDto.NegocioID}", negocioDto);
            })
            .WithName("CreateNegocio")
            .WithOpenApi()
            .WithSummary("🔐 Crear nuevo negocio")
            .WithDescription("Crea un nuevo negocio. Requiere autenticación.");
        }
    }
}

