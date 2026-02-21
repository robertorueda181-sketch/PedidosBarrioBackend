using MediatR;
using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.Commands.SaveEmpresaSede;
using PedidosBarrio.Application.Commands.UploadEmpresaLogo;
using PedidosBarrio.Application.Commands.UploadEmpresaProfileImage;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Queries.GetEmpresaSedeDetalle;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Api.EndPoint
{
    public static class EmpresaEndpoint
    {
        public static void MapEmpresaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/Empresa")
                           .WithTags("Empresa y Sede")
                           .RequireAuthorization();

            // GET /api/Empresa/sede
            group.MapGet("/sede", async (IMediator mediator, ICurrentUserService currentUserService) =>
            {
                var empresaId = currentUserService.GetEmpresaId();
                var query = new GetEmpresaSedeDetalleQuery(empresaId);
                var result = await mediator.Send(query);
                return result is not null ? Results.Ok(result) : Results.NotFound();
            })
            .WithName("GetEmpresaSedeDetalle")
            .WithOpenApi()
                        .WithSummary("🏢 Obtener detalles de la empresa y su sede principal")
                        .WithDescription("Obtiene el nombre, descripción, logo, redes sociales, teléfonos y dirección de la empresa del usuario logueado.");

                        // POST /api/Empresa/sede (Upsert)
                        group.MapPost("/sede", async ([FromBody] SaveEmpresaSedeDto dto, IMediator mediator, ICurrentUserService currentUserService) =>
                        {
                            var empresaId = currentUserService.GetEmpresaId();
                            var command = new SaveEmpresaSedeCommand(empresaId, dto);
                            var result = await mediator.Send(command);
                            return result ? Results.Ok(new { success = true, message = "Datos guardados correctamente" }) : Results.BadRequest();
                        })
                        .WithName("SaveEmpresaSede")
                        .WithOpenApi()
                        .WithSummary("💾 Guardar detalles de la empresa y sede")
                        .WithDescription("Crea o actualiza los datos de la empresa (redes, teléfonos, descripción) y su sede (dirección, ubicación).");

          
                                    // POST /api/Empresa/profile-image (Upload profile image with validation and optimization)
                                        group.MapPost("/profile-image", async (IFormFile file, IMediator mediator, ICurrentUserService currentUserService) =>
                                        {
                                            if (file == null || file.Length == 0)
                                            {
                                                return Results.BadRequest(new { success = false, message = "Archivo requerido" });
                                            }

                                            var empresaId = currentUserService.GetEmpresaId();

                                            using (var stream = file.OpenReadStream())
                                            {
                                                var command = new UploadEmpresaProfileImageCommand(empresaId, stream, file.FileName);
                                                var result = await mediator.Send(command);
                                                return result.Success ? Results.Ok(result) : Results.BadRequest(result);
                                            }
                                        })
                                        .WithName("UploadEmpresaProfileImage")
                                        .WithOpenApi()
                                        .WithSummary("👤 Subir imagen de perfil de empresa")
                                        .WithDescription("Sube y optimiza la imagen de perfil de la empresa. Valida la extensión, optimiza la imagen y devuelve la ruta. Formatos permitidos: JPG, JPEG, PNG, GIF, WebP. Máximo 5MB.")
                                        .Accepts<IFormFile>("multipart/form-data")
                                        .Produces<UploadEmpresaLogoResponseDto>(StatusCodes.Status200OK)
                                        .Produces(StatusCodes.Status400BadRequest)
                                        .DisableAntiforgery();

                                        // GET /api/Empresa/pasos-iniciales (Check if company has pending onboarding steps)
                                        group.MapGet("/pasos-iniciales", async (IMediator mediator, ICurrentUserService currentUserService, IPasoInicialRepository pasoRepository) =>
                                        {
                                            var empresaId = currentUserService.GetEmpresaId();

                                            var pasos = await pasoRepository.GetPasosPorEmpresaAsync(empresaId);
                                            var pasosList = pasos.ToList();

                                            var totalPasos = pasosList.Count;
                                            var pasosCompletados = pasosList.Count(p => p.Completado);
                                            var pasosPendientes = totalPasos - pasosCompletados;

                                            var response = new PasosPendientesDto
                                            {
                                                TienePasosPendientes = pasosPendientes > 0,
                                                TotalPasos = totalPasos,
                                                PasosCompletados = pasosCompletados,
                                                PasosPendientes = pasosPendientes
                                            };

                                            return Results.Ok(new { success = true, data = response });
                                        })
                                        .WithName("GetPasosPendientes")
                                        .WithOpenApi()
                                        .WithSummary("📋 Verificar pasos iniciales pendientes")
                                        .WithDescription("Verifica si la empresa tiene pasos iniciales pendientes de completar. Devuelve el total de pasos, completados y pendientes.");
                                    }
                            }
                        }
