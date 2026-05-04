using MediatR;
using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.Commands.CreatePresentacion;
using PedidosBarrio.Application.Commands.ImportarProductosMasivos;
using PedidosBarrio.Application.Commands.CargaMasivaImagenes;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Api.Services;
using PedidosBarrio.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace PedidosBarrio.Api.EndPoint
{
    public static class PresentacionEndpoint
    {
        public static void MapPresentacionEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/Presentaciones")
                           .WithTags("Presentaciones")
                           .RequireAuthorization();

            // ===== CREAR PRESENTACIÓN CON OPCIONES =====
            group.MapPost("/crear", async (
                [FromBody] CreatePresentacionDto dto,
                IMediator mediator) =>
            {
                var command = new CreatePresentacionCommand(dto);
                var result = await mediator.Send(command);
                return Results.Created($"/api/Presentaciones/{result.PresentacionID}", result);
            })
            .WithName("CreatePresentacion")
            .WithOpenApi()
            .WithSummary("✅ Crear presentación con opciones")
            .WithDescription("Crea una presentación (ej: Talla, Color) junto con sus opciones (S, M, L o Rojo, Verde, Azul)");

            // ===== DESCARGAR PLANTILLA EXCEL =====
            group.MapGet("/descargar-plantilla", async (
                IWebHostEnvironment env) =>
            {
                try
                {
                    var rutaPlantilla = Path.Combine(env.ContentRootPath, "plantillas", "plantilla_producto.xlsx");

                    if (!File.Exists(rutaPlantilla))
                        return Results.NotFound(new { error = $"Archivo no encontrado: {rutaPlantilla}" });

                    var bytes = await File.ReadAllBytesAsync(rutaPlantilla);
                    return Results.File(bytes, 
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Plantilla_Producto.xlsx");
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { error = $"Error al descargar la plantilla: {ex.Message}" });
                }
            })
            .WithName("DescargarPlantillaExcel")
            .WithOpenApi()
            .WithSummary("📥 Descargar plantilla Excel")
            .WithDescription("Descarga la plantilla Excel para importar productos, precios y presentaciones");

            // ===== IMPORTAR PRESENTACIONES DESDE EXCEL (ARCHIVO CARGADO) =====
            group.MapPost("/importar-excel", async (
                IFormFile archivo,
                IMediator mediator,
                ICurrentUserService currentUserService,
                IExcelSecurityService securityService) =>
            {
                try
                {
                    if (archivo == null || archivo.Length == 0)
                        return Results.BadRequest(new { error = "Debe proporcionar un archivo Excel válido" });

                    // Validar seguridad del archivo
                    var (esValido, errorMensaje) = await securityService.ValidarArchivoAsync(archivo);
                    if (!esValido)
                        return Results.BadRequest(new { error = $"Validación de seguridad fallida: {errorMensaje}" });

                    // Extraer contenido de forma segura
                    using var stream = await securityService.ExtraerContenidoSeguroAsync(archivo);

                    var empresaId = currentUserService.GetEmpresaId();
                    var command = new ImportarProductosMasivosCommand(stream, archivo.FileName, empresaId);
                    var result = await mediator.Send(command);

                    if (result.Exitoso)
                    {
                        return Results.Ok(result);
                    }
                    else
                    {
                        return Results.BadRequest(result);
                    }
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new 
                    { 
                        error = $"Error al procesar el archivo: {ex.Message}",
                        detalles = ex.InnerException?.Message 
                    });
                }
            })
            .WithName("ImportarProductosMasivos")
            .WithOpenApi()
            .WithSummary("📤 Importar productos masivamente desde Excel")
            .WithDescription("Importa productos, precios y presentaciones desde un archivo Excel. Incluye validaciones de seguridad contra virus y macros")
            .DisableAntiforgery();

            // ===== CARGA MASIVA DE IMÁGENES =====
            group.MapPost("/carga-imagenes", async (
                [FromForm] IFormCollection form,
                IMediator mediator,
                ICurrentUserService currentUserService,
                ILogger<Program> logger) =>
            {
                try
                {
                    var empresaId = currentUserService.GetEmpresaId();
                    var files = form.Files;

                    if (files == null || files.Count == 0)
                        return Results.BadRequest(new { error = "Debe proporcionar al menos un archivo de imagen" });

                    var imagenesDto = files.Select(f => new ArchivoImagenDto
                    {
                        Stream = f,
                        FileName = f.FileName,
                        Length = f.Length
                    }).ToList();

                    var command = new CargaMasivaImagenesCommand(imagenesDto, empresaId);
                    var result = await mediator.Send(command);

                    if (!result.Exitoso)
                    {
                        return Results.BadRequest(result);
                    }

                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error en carga masiva de imágenes");
                    return Results.Problem(
                        title: "Error interno del servidor",
                        detail: "Ocurrió un error inesperado durante la carga masiva de imágenes.",
                        statusCode: StatusCodes.Status500InternalServerError
                    );
                }
            })
            .WithName("CargaMasivaImagenes")
            .WithOpenApi()
            .WithSummary("📷 Carga masiva de imágenes para opciones de presentación")
            .WithDescription("Sube múltiples imágenes en formato WebP/PNG/JPG. Los nombres deben seguir el formato: codigo-variante1-variante2 (ej: PROD001-TallaM-ColorRojo). Las imágenes se optimizan automáticamente y se asignan a las opciones correspondientes.")
            .DisableAntiforgery();
        }
    }
}

