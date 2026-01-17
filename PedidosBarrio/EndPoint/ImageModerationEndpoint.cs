using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.Commands.ValidateImage;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Api.EndPoint
{
    public static class ImageModerationEndpoint
    {
        public static void MapImageModerationEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/ImageModeration")
                           .WithTags("Image Moderation")
                           .AllowAnonymous(); // No requiere autenticación

            // POST /api/ImageModeration/validate - Validar imagen usando URL
            group.MapPost("/validate", async (
                [FromBody] ImageValidationRequestDto request,
                IMediator mediator,
                IValidator<ImageValidationRequestDto> validator) =>
            {
                // Validar el request
                var validationResult = await validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Results.BadRequest(new
                    {
                        error = "Datos de entrada inválidos",
                        errors = validationResult.Errors.Select(e => new
                        {
                            field = e.PropertyName,
                            message = e.ErrorMessage
                        })
                    });
                }

                var command = new ValidateImageCommand(
                    request.ImageUrl,
                    request.Base64Image,
                    request.ToleranceLevel?.ToUpper() ?? "MEDIUM");

                var result = await mediator.Send(command);
                return Results.Ok(result);
            })
            .WithName("ValidateImage")
            .WithOpenApi()
            .WithSummary("🌍 Valida si una imagen es apropiada (Público)")
            .WithDescription(@"
                🚀 ENDPOINT PÚBLICO - No requiere autenticación JWT
                
                Utiliza Google Vision API para detectar contenido inapropiado en imágenes.
                
                **Parámetros:**
                - ImageUrl: URL de la imagen a validar
                - Base64Image: Imagen codificada en base64 (alternativa a ImageUrl)  
                - ToleranceLevel: Nivel de tolerancia (LOW, MEDIUM, HIGH)
                
                **Respuesta:**
                - IsAppropriate: true si la imagen es apropiada
                - ConfidenceScore: Nivel de confianza (0-1)
                - ViolationReasons: Lista de razones de violación
                - Details: Detalles específicos de la detección");

            // POST /api/ImageModeration/validate-url - Método alternativo solo con URL
            group.MapPost("/validate-url", async (
                [FromQuery] string imageUrl,
                [FromQuery] string toleranceLevel,
                IMediator mediator) =>
            {
                if (string.IsNullOrEmpty(imageUrl))
                {
                    return Results.BadRequest(new { error = "imageUrl es requerido" });
                }

                var command = new ValidateImageCommand(
                    imageUrl,
                    null,
                    toleranceLevel?.ToUpper() ?? "MEDIUM");

                var result = await mediator.Send(command);
                return Results.Ok(result);
            })
            .WithName("ValidateImageByUrl")
            .WithOpenApi()
            .WithSummary("🌍 Valida imagen por URL (Público - método simplificado)")
            .WithDescription("ENDPOINT PÚBLICO - Valida una imagen usando solo la URL como query parameter");

            // GET /api/ImageModeration/config - Obtener configuración disponible
            group.MapGet("/config", () =>
            {
                return Results.Ok(new
                {
                    toleranceLevels = new[]
                    {
                        new { value = "LOW", description = "Tolerancia baja - Detecta contenido posiblemente inapropiado" },
                        new { value = "MEDIUM", description = "Tolerancia media - Detecta contenido probablemente inapropiado (por defecto)" },
                        new { value = "HIGH", description = "Tolerancia alta - Solo detecta contenido definitivamente inapropiado" }
                    },
                    supportedFormats = new[] { "JPEG", "PNG", "GIF", "BMP", "WEBP", "ICO" },
                    maxImageSize = "20MB",
                    detectionTypes = new[]
                    {
                        "Contenido adulto",
                        "Contenido violento", 
                        "Contenido médico",
                        "Contenido sugerente",
                        "Contenido falso/manipulado"
                    }
                });
            })
            .WithName("GetImageModerationConfig")
            .WithOpenApi()
            .WithSummary("🌍 Obtiene la configuración disponible (Público)")
            .WithDescription("ENDPOINT PÚBLICO - Retorna los niveles de tolerancia y tipos de detección soportados");
        }
    }
}