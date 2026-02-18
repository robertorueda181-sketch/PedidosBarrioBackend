using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Queries.GetPasosIniciales;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Api.EndPoint;

public static class PasoInicialEndpoint
{
    public static void MapPasoInicialEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/PasosIniciales")
                       .WithTags("Pasos Iniciales")
                       .RequireAuthorization();

        // GET /api/PasosIniciales - Obtener pasos iniciales de la empresa del token
        group.MapGet("/", async (
            HttpContext httpContext,
            IMediator mediator) =>
        {
            try
            {
                // Obtener EmpresaID del token
                var empresaIdClaim = httpContext.User.FindFirst("EmpresaID");
                if (empresaIdClaim == null || !Guid.TryParse(empresaIdClaim.Value, out var empresaId))
                {
                    return Results.Unauthorized();
                }

                var query = new GetPasosInicialesQuery(empresaId);
                var result = await mediator.Send(query);
                return Results.Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { success = false, message = ex.Message });
            }
        })
        .WithName("GetPasosIniciales")
        .WithOpenApi()
        .WithSummary("📋 Obtener pasos iniciales de la empresa")
        .WithDescription("Obtiene todos los pasos iniciales para completar el setup de la empresa.")
        .Produces<IEnumerable<PasoInicialDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized);

        // PUT /api/PasosIniciales/{pasoId} - Marcar paso como completado
        group.MapPut("/{pasoId}", async (
            int pasoId,
            [FromBody] ActualizarPasoInicialDto dto,
            IPasoInicialRepository repository) =>
        {
            try
            {
                var paso = await repository.GetByIdAsync(pasoId);
                if (paso == null)
                {
                    return Results.NotFound(new { success = false, message = "Paso no encontrado" });
                }

                paso.Completado = dto.Completado;
                if (dto.Completado && !paso.FechaCompletado.HasValue)
                {
                    paso.FechaCompletado = DateTime.UtcNow;
                }

                await repository.UpdateAsync(paso);

                return Results.Ok(new 
                { 
                    success = true, 
                    message = "Paso actualizado exitosamente",
                    data = new PasoInicialDto
                    {
                        PasoID = paso.PasoID,
                        EmpresaID = paso.EmpresaID,
                        Titulo = paso.Titulo,
                        Descripcion = paso.Descripcion,
                        Icono = paso.Icono,
                        Ruta = paso.Ruta,
                        Obligatorio = paso.Obligatorio,
                        Completado = paso.Completado,
                        Orden = paso.Orden,
                        FechaCreacion = paso.FechaCreacion,
                        FechaCompletado = paso.FechaCompletado
                    }
                });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { success = false, message = ex.Message });
            }
        })
        .WithName("ActualizarPasoInicial")
        .WithOpenApi()
        .WithSummary("✅ Marcar paso como completado")
        .WithDescription("Actualiza el estado de completación de un paso inicial.")
        .Accepts<ActualizarPasoInicialDto>("application/json")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status400BadRequest);
    }
}

