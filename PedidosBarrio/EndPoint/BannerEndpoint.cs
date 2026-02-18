using MediatR;
using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.Commands.CreateBanner;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Api.EndPoint
{
    public static class BannerEndpoint
    {
        public static void MapBannerEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/Banner")
                           .WithTags("Banner")
                           .RequireAuthorization();

            // POST /api/Banner - Crear banner con imagen y validación de IA
            group.MapPost("/", async (
                [FromForm] string? titulo,
                [FromForm] string? descripcion,
                [FromForm] string? textoBoton,
                [FromForm] string? link,
                [FromForm] string? redireccion,
                [FromForm] DateTime fechaInicio,
                [FromForm] DateTime fechaFin,
                [FromForm] DateTime fechaExpiracion,
                [FromForm] IFormFile? imagen,
                IMediator mediator,
                ICurrentUserService currentUserService) =>
            {
                try
                {
                    // Obtener empresaId del token del usuario autenticado
                    var empresaId = currentUserService.GetEmpresaId();

                    if (fechaInicio >= fechaFin)
                    {
                        return Results.BadRequest(new { success = false, message = "FechaInicio debe ser menor a FechaFin" });
                    }

                    Stream? imagenStream = null;
                    string? imagenFileName = null;

                    if (imagen != null && imagen.Length > 0)
                    {
                        imagenStream = imagen.OpenReadStream();
                        imagenFileName = imagen.FileName;
                    }

                    // Usar el nuevo comando con validación de IA, verificación de duplicados y prioridad por suscripción
                    var command = new CreateBannerWithValidationCommand(
                        empresaId,
                        titulo,
                        descripcion,
                        textoBoton,
                        link,
                        redireccion,
                        fechaInicio,
                        fechaFin,
                        fechaExpiracion,
                        imagenStream,
                        imagenFileName);

                    var result = await mediator.Send(command);

                    imagenStream?.Dispose();

                    return result.Success ? Results.Ok(result) : Results.BadRequest(result);
                }
                catch (UnauthorizedAccessException ex)
                {
                    return Results.Unauthorized();
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { success = false, message = ex.Message });
                }
            })
            .WithName("CreateBanner")
            .WithOpenApi()
            .WithSummary("📸 Crear banner con imagen")
            .WithDescription("Crea un nuevo banner con validación de IA, detección de duplicados y prioridad automática según nivel de suscripción. El empresaId se obtiene del token autenticado. Niveles: 1=Premium (Aprobado automático), 2=Plus, 3=Basic")
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<BannerResponseDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .DisableAntiforgery();

            // GET /api/Banner/empresa - Obtener banners activos de la empresa del usuario
            group.MapGet("/", async (
                IBannerRepository bannerRepository,
                ICurrentUserService currentUserService) =>
            {
                try
                {
                    var empresaId = currentUserService.GetEmpresaId();
                    var banners = await bannerRepository.GetActiveByEmpresaIdAsync(empresaId);
                    return banners.Any() ? Results.Ok(banners) : Results.NotFound(new { message = "No banners found" });
                }
                catch (UnauthorizedAccessException)
                {
                    return Results.Unauthorized();
                }
            })
            .WithName("GetBannersByEmpresa")
            .WithOpenApi()
            .WithSummary("🏢 Obtener banners activos de mi empresa")
            .WithDescription("Obtiene todos los banners activos de la empresa del usuario autenticado, ordenados por prioridad.");

            // GET /api/Banner/{id} - Obtener un banner específico
            group.MapGet("/{id}", async (
                int id,
                IBannerRepository bannerRepository) =>
            {
                try
                {
                    var banner = await bannerRepository.GetByIdAsync(id);
                    return Results.Ok(banner);
                }
                catch (KeyNotFoundException)
                {
                    return Results.NotFound(new { message = $"Banner {id} not found" });
                }
            })
            .WithName("GetBannerById")
            .WithOpenApi()
            .WithSummary("🔍 Obtener banner por ID")
            .WithDescription("Obtiene los detalles de un banner específico");

            // PUT /api/Banner/{id} - Actualizar banner
            group.MapPut("/{id}", async (
                int id,
                [FromBody] CreateBannerDto dto,
                IBannerRepository bannerRepository) =>
            {
                try
                {
                    var banner = await bannerRepository.GetByIdAsync(id);
                    banner.Titulo = dto.Titulo;
                    banner.Descripcion = dto.Descripcion;
                    banner.TextoBoton = dto.TextoBoton;
                    banner.Link = dto.Link;
                    banner.FechaInicio = dto.FechaInicio;
                    banner.FechaExpiracion = dto.FechaExpiracion;
                    banner.Visible = dto.Visible;
                    banner.Aprobado = dto.Aprobado;
                    banner.Prioridad = dto.Prioridad;

                    await bannerRepository.UpdateAsync(banner);

                    return Results.Ok(new { success = true, message = "Banner actualizado correctamente" });
                }
                catch (KeyNotFoundException)
                {
                    return Results.NotFound(new { success = false, message = $"Banner {id} not found" });
                }
            })
            .WithName("UpdateBanner")
            .WithOpenApi()
            .WithSummary("✏️ Actualizar banner")
            .WithDescription("Actualiza los datos de un banner (sin imagen).");

            // DELETE /api/Banner/{id} - Eliminar banner
            group.MapDelete("/{id}", async (
                int id,
                IBannerRepository bannerRepository) =>
            {
                try
                {
                    await bannerRepository.DeleteAsync(id);
                    return Results.Ok(new { success = true, message = "Banner eliminado correctamente" });
                }
                catch (KeyNotFoundException)
                {
                    return Results.NotFound(new { success = false, message = $"Banner {id} not found" });
                }
            })
            .WithName("DeleteBanner")
            .WithOpenApi()
            .WithSummary("🗑️ Eliminar banner")
            .WithDescription("Desactiva un banner (eliminación lógica)");
        }
    }
}
