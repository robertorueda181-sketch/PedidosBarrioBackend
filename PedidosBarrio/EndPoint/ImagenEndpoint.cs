using MediatR;
using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.Commands.DeleteImagen;
using PedidosBarrio.Application.Commands.UploadImage;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Queries.GetAllImagenes;
using PedidosBarrio.Application.Queries.GetImagenById;
using PedidosBarrio.Application.Queries.GetImagenesByProducto;
using PedidosBarrio.Application.Services;

namespace PedidosBarrio.Api.EndPoint
{
    public static class ImagenEndpoint
    {
        public static void MapImagenEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/Imagenes")
                           .WithTags("Imagenes");

            // GET /api/Imagenes
            group.MapGet("/", async (IMediator mediator) =>
            {
                var imagenes = await mediator.Send(new GetAllImagenesQuery());
                return Results.Ok(imagenes);
            })
            .WithName("GetAllImagenes")
            .WithOpenApi();

            // GET /api/Imagenes/{id}
            group.MapGet("/{id:int}", async (int id, IMediator mediator) =>
            {
                var imagen = await mediator.Send(new GetImagenByIdQuery(id));
                return imagen is not null ? Results.Ok(imagen) : Results.NotFound();
            })
            .WithName("GetImagenById")
            .WithOpenApi();

            // GET /api/Imagenes/producto/{productoId}
            group.MapGet("/id/{externalId:int}", async (int externalId, string tipo, IMediator mediator) =>
            {
                var imagenes = await mediator.Send(new GetImagenesByProductoQuery(externalId,tipo));
                return Results.Ok(imagenes);
            })
            .WithName("GetImagenesById")
            .WithOpenApi();

            // POST /api/Imagenes/upload
            group.MapPost("/upload", async (
                IFormFile file,
                [FromForm] int productoId,
                [FromForm] string? descripcion,
                [FromForm] bool setAsPrincipal,
                IMediator mediator) =>
            {
                if (file == null || file.Length == 0)
                    return Results.BadRequest("No se ha proporcionado ningún archivo.");

                var command = new UploadImageCommand(productoId, descripcion, setAsPrincipal, file, file.FileName);
                var result = await mediator.Send(command);
                return Results.Created($"/api/Imagenes/{result.ImagenID}", result);
            })
            .DisableAntiforgery()
            .WithName("UploadImage")
            .WithOpenApi()
            .WithSummary("📤 Subir una imagen de producto")
            .WithDescription("Sube una imagen, la optimiza (convierte a WebP y comprime) y la asocia a un producto.");

            // DELETE /api/Imagenes/{id}
            group.MapDelete("/{id:int}", async (int id, IMediator mediator) =>
            {
                await mediator.Send(new DeleteImagenCommand(id));
                return Results.NoContent();
            })
            .WithName("DeleteImagen")
            .WithOpenApi();

            // POST /api/Imagenes/optimize - Nuevo endpoint genérico de optimización
            group.MapPost("/optimize", async (
                IFormFile imagen,
                [FromForm] string tipo,
                IImageSaveStrategyFactory strategyFactory) =>
            {
                if (imagen == null || imagen.Length == 0)
                    return Results.BadRequest(new { error = "No se ha proporcionado ninguna imagen." });

                // Validar y parsear el tipo de imagen
                if (!Enum.TryParse<ImageType>(tipo, true, out var imageType))
                {
                    var tiposValidos = string.Join(", ", Enum.GetNames<ImageType>());
                    return Results.BadRequest(new 
                    { 
                        error = $"Tipo de imagen inválido: '{tipo}'. Tipos válidos: {tiposValidos}" 
                    });
                }

                try
                {
                    // Obtener la estrategia correspondiente
                    var strategy = strategyFactory.GetStrategy(imageType);

                    // Procesar y guardar la imagen
                    using var stream = imagen.OpenReadStream();
                    var imageUrl = await strategy.SaveImageAsync(stream, imagen.FileName);

                    // Obtener dimensiones según el tipo
                    var dimensiones = imageType switch
                    {
                        ImageType.Banner => "1200x600",
                        ImageType.Producto => "400x400",
                        ImageType.Empresa => "300x300",
                        ImageType.Categoria => "500x500",
                        ImageType.Avatar => "200x200",
                        ImageType.Original => "Original (sin redimensionar)",
                        _ => "Desconocido"
                    };

                    var response = new OptimizeImageResponseDto
                    {
                        Url = imageUrl,
                        TipoImagen = imageType.ToString(),
                        Dimensiones = dimensiones,
                        Formato = "webp",
                        Mensaje = $"Imagen optimizada y guardada exitosamente como {imageType}"
                    };

                    return Results.Ok(response);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
                catch (ApplicationException ex)
                {
                    return Results.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError,
                        title: "Error al procesar la imagen"
                    );
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        detail: $"Error inesperado: {ex.Message}",
                        statusCode: StatusCodes.Status500InternalServerError,
                        title: "Error interno del servidor"
                    );
                }
            })
            .DisableAntiforgery()
            .WithName("OptimizeImage")
            .WithOpenApi()
            .Produces<OptimizeImageResponseDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("🖼️ Optimizar y guardar imagen genérica")
            .WithDescription(@"Optimiza una imagen según el tipo especificado, la convierte a WebP y retorna la URL.

**Tipos de imagen soportados:**
- **Banner**: 1200x600 (para banners y cabeceras)
- **Producto**: 400x400 (para imágenes de productos)
- **Empresa**: 300x300 (para logos de empresas)
- **Categoria**: 500x500 (para imágenes de categorías)
- **Avatar**: 200x200 (para fotos de perfil)
- **Original**: Sin redimensionar, solo convierte a WebP

**Parámetros:**
- `imagen` (file): Archivo de imagen a procesar
- `tipo` (string): Tipo de imagen (Banner, Producto, Empresa, Categoria, Avatar, Original)

**Formato de salida:** Todas las imágenes se convierten a formato WebP optimizado.");
        }
    }
}
