using MediatR;
using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.Commands.CreatePresentacion;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Services;

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
            group.MapGet("/descargar-plantilla", async (IPresentacionExcelService excelService) =>
            {
                var bytes = await excelService.GenerarPlantillaAsync();
                return Results.File(bytes, 
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Plantilla_Presentaciones.xlsx");
            })
            .WithName("DescargarPlantillaExcel")
            .WithOpenApi()
            .WithSummary("📥 Descargar plantilla Excel")
            .WithDescription("Descarga una plantilla Excel para importar presentaciones masivamente");

            // ===== IMPORTAR PRESENTACIONES DESDE EXCEL =====
            group.MapPost("/importar-excel", async (
                IFormFile archivo,
                IMediator mediator,
                IPresentacionExcelService excelService) =>
            {
                if (archivo == null || archivo.Length == 0)
                {
                    return Results.BadRequest(new { error = "Archivo no válido" });
                }

                if (!archivo.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest(new { error = "Solo se aceptan archivos .xlsx" });
                }

                try
                {
                    using (var stream = archivo.OpenReadStream())
                    {
                        var filas = await excelService.LeerPresentacionesAsync(stream);

                        if (!filas.Any())
                        {
                            return Results.BadRequest(new { error = "El archivo no contiene datos válidos" });
                        }

                        // Agrupar por producto y presentación
                        var presentacionesAgrupadas = filas
                            .GroupBy(f => new { f.ProductoID, f.NombrePresentacion })
                            .ToDictionary(
                                g => g.Key,
                                g => g.Select(f => new CreatePresentacionOpcionDto
                                {
                                    Valor = f.ValorOpcion,
                                    Precio = f.PrecioOpcion,
                                    Imagen = f.ImagenOpcion,
                                    Stock = f.StockOpcion,
                                    Descripcion = f.DescripcionOpcion
                                }).ToList()
                            );

                        var resultados = new List<PresentacionDetalleDto>();
                        var errores = new List<string>();

                        // Crear presentaciones
                        foreach (var (key, opciones) in presentacionesAgrupadas)
                        {
                            try
                            {
                                var command = new CreatePresentacionCommand(
                                    new CreatePresentacionDto
                                    {
                                        Descripcion = key.NombrePresentacion,
                                        ProductoID = key.ProductoID,
                                        Opciones = opciones
                                    }
                                );

                                var result = await mediator.Send(command);
                                resultados.Add(result);
                            }
                            catch (Exception ex)
                            {
                                errores.Add($"Error al crear presentación '{key.NombrePresentacion}' para producto {key.ProductoID}: {ex.Message}");
                            }
                        }

                        return Results.Ok(new
                        {
                            exitosos = resultados.Count,
                            errores = errores,
                            presentaciones = resultados
                        });
                    }
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { error = $"Error al procesar el archivo: {ex.Message}" });
                }
            })
            .Accepts<IFormFile>("multipart/form-data")
            .WithName("ImportarPresentacionesExcel")
            .WithOpenApi()
            .WithSummary("📤 Importar presentaciones desde Excel")
            .WithDescription("Importa presentaciones y opciones de forma masiva desde un archivo Excel");
        }
    }
}
