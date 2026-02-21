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
            // Grupo SIN autenticación requerida
            var publicGroup = app.MapGroup("/api/Banner")
                           .WithTags("Banner");

            // GET /api/Banner/publicos - Obtener todos los banners activos (públicos)
            publicGroup.MapGet("/publicos", async (
                IBannerRepository bannerRepository) =>
            {
                try
                {
                    var banners = await bannerRepository.GetAllActiveAsync();
                    return Results.Ok(banners);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { success = false, message = ex.Message });
                }
            })
            .WithName("GetAllActiveBanners")
            .WithOpenApi()
            .WithSummary("🌍 Obtener todos los banners activos")
            .WithDescription("Obtiene todos los banners activos de todas las empresas, sin autenticación requerida. Ordenados por prioridad y estado de aprobación.");

            // Grupo CON autenticación requerida
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
                        return Results.BadRequest(new { success = false, message = "Fecha Inicio debe ser menor a Fecha Fin" });
                    }

                    Stream? imagenStream = null;
                    string? imagenFileName = null;

                    if (imagen != null && imagen.Length > 0)
                    {
                        imagenStream = imagen.OpenReadStream();
                        imagenFileName = imagen.FileName;
                    }

                    // Usar el nuevo comando con valiación de IA, verificación de duplicados y prioridad por suscripción
                    var command = new CreateBannerWithValidationCommand(
                        empresaId,
                        titulo,
                        descripcion,
                        textoBoton,
                        link,
                        redireccion,
                        fechaInicio,
                        fechaFin,
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
                    return Results.Ok(banners);
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
            group.MapGet("/{id:guid}", async (
                Guid id,
                IBannerRepository bannerRepository,
                ICurrentUserService currentUserService) =>
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
            group.MapPut("/{id:guid}", async (
                Guid id,
                [FromForm] string? titulo,
                [FromForm] string? descripcion,
                [FromForm] string? textoBoton,
                [FromForm] string? link,
                [FromForm] string? redireccion,
                [FromForm] DateTime fechaInicio,
                [FromForm] DateTime fechaFin,
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
                        return Results.BadRequest(new { success = false, message = "Fecha Inicio debe ser menor a Fecha Fin" });
                    }

                    Stream? imagenStream = null;
                    string? imagenFileName = null;

                    if (imagen != null && imagen.Length > 0)
                    {
                        imagenStream = imagen.OpenReadStream();
                        imagenFileName = imagen.FileName;
                    }


                    var command = new UpdateBannerWithValidationCommand(
                      id,
                      empresaId,
                      titulo,
                      descripcion,
                      textoBoton,
                      link,
                      redireccion,
                      fechaInicio,
                      fechaFin,
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
            .WithName("UpdateBanner")
            .WithOpenApi()
            .WithSummary("✏️ Actualizar banner")
            .WithDescription("Actualiza los datos de un banner. Requiere ser propietario del banner.")
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<BannerResponseDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .DisableAntiforgery();

            // DELETE /api/Banner/{id} - Eliminar banner
            group.MapDelete("/{id:guid}", async (
                Guid id,
                IBannerRepository bannerRepository,
                ICurrentUserService currentUserService) =>
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
