using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.Commands.ModerateText;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Services;

namespace PedidosBarrio.Api.EndPoint
{
    public static class TextModerationEndpoint
    {
        public static void MapTextModerationEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/TextModeration")
                           .WithTags("Text Moderation")
                           .AllowAnonymous(); // Público, sin autenticación

            // POST /api/TextModeration/validate - Validar texto
            group.MapPost("/validate", async (
                [FromBody] TextModerationRequestDto request,
                IMediator mediator,
                IValidator<TextModerationRequestDto> validator) =>
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

                var command = new ModerateTextCommand(
                    request.Text,
                    request.Model ?? "omni-moderation-latest");

                var result = await mediator.Send(command);
                return Results.Ok(result);
            })
            .WithName("ModerateText")
            .WithOpenApi()
            .WithSummary("🌍 Modera contenido de texto (Público)")
            .WithDescription(@"
                🚀 ENDPOINT PÚBLICO - No requiere autenticación JWT
                
                Utiliza OpenAI Moderation API para detectar contenido inapropiado en texto.
                
                **Parámetros:**
                - Text: Texto a moderar (máximo 10,000 caracteres)
                - Model: Modelo a usar (omni-moderation-latest, text-moderation-stable)
                
                **Detecta:**
                - Contenido sexual
                - Discurso de odio  
                - Acoso y amenazas
                - Autolesiones
                - Violencia
                - Y más categorías...
                
                **Respuesta:**
                - IsAppropriate: true si el texto es apropiado
                - Flagged: true si se detectó contenido problemático
                - ViolationCategories: Categorías específicas detectadas
                - HighestScore: Score más alto (0.0-1.0)");

            // POST /api/TextModeration/validate-simple - Método simplificado
            group.MapPost("/validate-simple", async (
                [FromQuery] string text,
                [FromQuery] string model,
                IMediator mediator) =>
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    return Results.BadRequest(new { error = "El parámetro 'text' es requerido" });
                }

                if (text.Length > 10000)
                {
                    return Results.BadRequest(new { error = "El texto no puede exceder los 10,000 caracteres" });
                }

                var command = new ModerateTextCommand(
                    text,
                    model ?? "omni-moderation-latest");

                var result = await mediator.Send(command);
                return Results.Ok(result);
            })
            .WithName("ModerateTextSimple")
            .WithOpenApi()
            .WithSummary("🌍 Modera texto (método simplificado)")
            .WithDescription("ENDPOINT PÚBLICO - Modera texto usando query parameters");

            // GET /api/TextModeration/info - Información sobre el servicio
            group.MapGet("/info", () =>
            {
                return Results.Ok(new
                {
                    service = "OpenAI Moderation API",
                    version = "2024-01",
                    models = new[]
                    {
                        new { 
                            name = "omni-moderation-latest", 
                            description = "Modelo más reciente de moderación multimodal",
                            recommended = true
                        },
                        new { 
                            name = "text-moderation-stable", 
                            description = "Modelo estable para uso en producción (DEPRECATED)",
                            recommended = false
                        }
                    },
                    categories = new[]
                    {
                        new { category = "sexual", description = "Contenido sexual" },
                        new { category = "hate", description = "Discurso de odio" },
                        new { category = "harassment", description = "Acoso" },
                        new { category = "self-harm", description = "Autolesiones" },
                        new { category = "sexual/minors", description = "Contenido sexual con menores" },
                        new { category = "hate/threatening", description = "Amenazas de odio" },
                        new { category = "violence/graphic", description = "Violencia gráfica" },
                        new { category = "self-harm/intent", description = "Intención de autolesión" },
                        new { category = "self-harm/instructions", description = "Instrucciones de autolesión" },
                        new { category = "harassment/threatening", description = "Acoso amenazante" },
                        new { category = "violence", description = "Violencia" }
                    },
                    limits = new
                    {
                        maxCharacters = 10000,
                        encoding = "UTF-8",
                        rateLimit = "Según límites de OpenAI"
                    }
                });
            })
            .WithName("GetTextModerationInfo")
            .WithOpenApi()
            .WithSummary("🌍 Información del servicio de moderación")
            .WithDescription("ENDPOINT PÚBLICO - Información sobre modelos y categorías disponibles");

            // GET /api/TextModeration/status - Estado del servicio
            group.MapGet("/status", async (ITextModerationService textService) =>
            {
                try
                {
                    // Hacer un request mínimo para verificar el estado
                    var testResult = await textService.ModerateTextAsync("test", "omni-moderation-latest");
                    
                    var status = testResult.ViolationCategories.Contains("RATE_LIMIT_EXCEEDED") ? "RATE_LIMITED" :
                                testResult.ViolationCategories.Contains("API_ERROR") ? "API_ERROR" : "HEALTHY";

                    return Results.Ok(new
                    {
                        status,
                        timestamp = DateTime.UtcNow,
                        message = status switch
                        {
                            "HEALTHY" => "Servicio funcionando correctamente",
                            "RATE_LIMITED" => "Servicio limitado por rate limit",
                            "API_ERROR" => "Error en la API de OpenAI",
                            _ => "Estado desconocido"
                        },
                        recommendations = status == "RATE_LIMITED" ? new[]
                        {
                            "Espera unos minutos antes del próximo request",
                            "Considera implementar cache para resultados repetidos",
                            "Usa el endpoint batch para múltiples textos"
                        } : Array.Empty<string>()
                    });
                }
                catch (Exception ex)
                {
                    return Results.Ok(new
                    {
                        status = "ERROR",
                        timestamp = DateTime.UtcNow,
                        message = $"Error al verificar estado: {ex.Message}"
                    });
                }
            })
            .WithName("GetTextModerationStatus")
            .WithOpenApi()
            .WithSummary("🌍 Estado del servicio de moderación")
            .WithDescription("ENDPOINT PÚBLICO - Verifica el estado actual del servicio de moderación");

            // POST /api/TextModeration/batch - Moderar múltiples textos
            group.MapPost("/batch", async (
                [FromBody] List<string> texts,
                [FromQuery] string model,
                IMediator mediator) =>
            {
                if (texts == null || !texts.Any())
                {
                    return Results.BadRequest(new { error = "Debe proporcionar al menos un texto" });
                }

                if (texts.Count > 10)
                {
                    return Results.BadRequest(new { error = "Máximo 10 textos por batch" });
                }

                var tasks = texts.Select(async text =>
                {
                    if (string.IsNullOrWhiteSpace(text) || text.Length > 10000)
                    {
                        return new TextModerationResponseDto
                        {
                            IsAppropriate = false,
                            Flagged = true,
                            Message = "Texto vacío o muy largo",
                            ViolationCategories = { "INVALID_INPUT" }
                        };
                    }

                    var command = new ModerateTextCommand(text, model ?? "omni-moderation-latest");
                    return await mediator.Send(command);
                });

                var results = await Task.WhenAll(tasks);

                return Results.Ok(new
                {
                    totalTexts = texts.Count,
                    appropriateCount = results.Count(r => r.IsAppropriate),
                    flaggedCount = results.Count(r => r.Flagged),
                    results = results.Select((result, index) => new
                    {
                        index,
                        text = texts[index].Length > 50 ? texts[index].Substring(0, 50) + "..." : texts[index],
                        isAppropriate = result.IsAppropriate,
                        flagged = result.Flagged,
                        violations = result.ViolationCategories,
                        highestScore = result.HighestScore
                    })
                });
            })
            .WithName("ModerateTextBatch")
            .WithOpenApi()
            .WithSummary("🌍 Modera múltiples textos (batch)")
            .WithDescription("ENDPOINT PÚBLICO - Modera hasta 10 textos en una sola petición");

            // POST /api/TextModeration/test-words - Probar palabras específicas para debugging
            group.MapPost("/test-words", async (
                [FromBody] string[] testWords,
                IMediator mediator) =>
            {
                if (testWords == null || !testWords.Any())
                {
                    return Results.BadRequest(new { error = "Debe proporcionar al menos una palabra para probar" });
                }

                var results = new List<object>();

                foreach (var word in testWords.Take(20)) // Máximo 20 palabras
                {
                    if (string.IsNullOrWhiteSpace(word)) continue;

                    var command = new ModerateTextCommand(word, "omni-moderation-latest");
                    var result = await mediator.Send(command);

                    results.Add(new
                    {
                        word = word,
                        isAppropriate = result.IsAppropriate,
                        flagged = result.Flagged,
                        highestScore = result.HighestScore,
                        violations = result.ViolationCategories,
                        message = result.Message,
                        details = new
                        {
                            sexual = result.Details.Sexual,
                            hate = result.Details.Hate,
                            harassment = result.Details.Harassment,
                            violence = result.Details.Violence
                        }
                    });
                }

                return Results.Ok(new
                {
                    totalWords = testWords.Length,
                    processedWords = results.Count,
                    flaggedWords = results.Count(r => ((dynamic)r).flagged),
                    appropriateWords = results.Count(r => ((dynamic)r).isAppropriate),
                    results = results
                });
            })
            .WithName("TestWordsModeration")
            .WithOpenApi()
            .WithSummary("🧪 Probar palabras específicas (DEBUG)")
            .WithDescription("ENDPOINT PÚBLICO - Para probar cómo se comporta el sistema con palabras específicas");
        }
    }
}