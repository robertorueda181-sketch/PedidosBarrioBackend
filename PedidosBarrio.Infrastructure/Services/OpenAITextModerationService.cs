using Microsoft.Extensions.Configuration;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Services;
using System.Net;
using System.Text;
using System.Text.Json;

namespace PedidosBarrio.Infrastructure.Services
{
    public class OpenAITextModerationService : ITextModerationService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;
        private const string OpenAiModerationUrl = "https://api.openai.com/v1/moderations";

        public OpenAITextModerationService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _apiKey = _configuration.GetSection("OpenAI:ApiKey").Value
                     ?? throw new InvalidOperationException("OpenAI API Key not configured in OpenAI:ApiKey");
        }

        public async Task<TextModerationResponseDto> ModerateTextAsync(string text, string model = "omni-moderation-latest")
        {
            const int maxRetries = 3;
            const int baseDelayMs = 1000; // 1 segundo

            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        return new TextModerationResponseDto
                        {
                            IsAppropriate = true,
                            Flagged = false,
                            Message = "Texto vacío - considerado apropiado"
                        };
                    }

                    var requestBody = new
                    {
                        input = text,
                        model = model
                    };

                    var jsonContent = JsonSerializer.Serialize(requestBody);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    // Configurar headers de autorización
                    _httpClient.DefaultRequestHeaders.Clear();
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                    var response = await _httpClient.PostAsync(OpenAiModerationUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        return ParseOpenAIModerationResponse(responseContent, text);
                    }
                    
                    // Manejo específico de Rate Limiting
                    if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                        if (attempt < maxRetries)
                        {
                            // Exponential backoff: 1s, 2s, 4s
                            var delay = baseDelayMs * (int)Math.Pow(2, attempt);
                            
                            // Verificar si hay header Retry-After
                            if (response.Headers.RetryAfter != null)
                            {
                                delay = (int)(response.Headers.RetryAfter.Delta?.TotalMilliseconds ?? delay);
                            }

                            await Task.Delay(delay);
                            continue; // Reintentar
                        }
                        else
                        {
                            return new TextModerationResponseDto
                            {
                                IsAppropriate = false,
                                Flagged = true,
                                Message = "⚠️ Límite de requests excedido. Intenta nuevamente en unos minutos.",
                                ViolationCategories = { "RATE_LIMIT_EXCEEDED" }
                            };
                        }
                    }
                    
                    // Otros errores HTTP
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new TextModerationResponseDto
                    {
                        IsAppropriate = false,
                        Flagged = true,
                        Message = $"Error de OpenAI API: {response.StatusCode} - {errorContent}",
                        ViolationCategories = { "API_ERROR" }
                    };
                }
                catch (TaskCanceledException)
                {
                    return new TextModerationResponseDto
                    {
                        IsAppropriate = false,
                        Flagged = true,
                        Message = "Timeout al conectar con OpenAI API",
                        ViolationCategories = { "TIMEOUT_ERROR" }
                    };
                }
                catch (Exception ex)
                {
                    if (attempt < maxRetries)
                    {
                        // En caso de error de red, esperar antes de reintentar
                        await Task.Delay(baseDelayMs * (attempt + 1));
                        continue;
                    }

                    return new TextModerationResponseDto
                    {
                        IsAppropriate = false,
                        Flagged = true,
                        Message = $"Error interno: {ex.Message}",
                        ViolationCategories = { "INTERNAL_ERROR" }
                    };
                }
            }

            // Si llegamos aquí, todos los reintentos fallaron
            return new TextModerationResponseDto
            {
                IsAppropriate = false,
                Flagged = true,
                Message = "Error: Se agotaron los reintentos",
                ViolationCategories = { "MAX_RETRIES_EXCEEDED" }
            };
        }

        private TextModerationResponseDto ParseOpenAIModerationResponse(string jsonResponse, string originalText)
        {
            try
            {
                using var document = JsonDocument.Parse(jsonResponse);
                var results = document.RootElement.GetProperty("results");

                if (results.GetArrayLength() == 0)
                {
                    return new TextModerationResponseDto
                    {
                        IsAppropriate = true,
                        Flagged = false,
                        Message = "No se pudo analizar el texto"
                    };
                }

                var firstResult = results[0];
                var flagged = firstResult.GetProperty("flagged").GetBoolean();
                var categories = firstResult.GetProperty("categories");
                var categoryScores = firstResult.GetProperty("category_scores");

                var details = new TextModerationDetails
                {
                    Sexual = GetBooleanProperty(categories, "sexual"),
                    Hate = GetBooleanProperty(categories, "hate"),
                    Harassment = GetBooleanProperty(categories, "harassment"),
                    SelfHarm = GetBooleanProperty(categories, "self-harm"),
                    SexualMinors = GetBooleanProperty(categories, "sexual/minors"),
                    HateThreatening = GetBooleanProperty(categories, "hate/threatening"),
                    ViolenceGraphic = GetBooleanProperty(categories, "violence/graphic"),
                    SelfHarmIntent = GetBooleanProperty(categories, "self-harm/intent"),
                    SelfHarmInstructions = GetBooleanProperty(categories, "self-harm/instructions"),
                    HarassmentThreatening = GetBooleanProperty(categories, "harassment/threatening"),
                    Violence = GetBooleanProperty(categories, "violence"),

                    SexualScore = GetDoubleProperty(categoryScores, "sexual"),
                    HateScore = GetDoubleProperty(categoryScores, "hate"),
                    HarassmentScore = GetDoubleProperty(categoryScores, "harassment"),
                    SelfHarmScore = GetDoubleProperty(categoryScores, "self-harm"),
                    SexualMinorsScore = GetDoubleProperty(categoryScores, "sexual/minors"),
                    HateThreateningScore = GetDoubleProperty(categoryScores, "hate/threatening"),
                    ViolenceGraphicScore = GetDoubleProperty(categoryScores, "violence/graphic"),
                    SelfHarmIntentScore = GetDoubleProperty(categoryScores, "self-harm/intent"),
                    SelfHarmInstructionsScore = GetDoubleProperty(categoryScores, "self-harm/instructions"),
                    HarassmentThreateningScore = GetDoubleProperty(categoryScores, "harassment/threatening"),
                    ViolenceScore = GetDoubleProperty(categoryScores, "violence")
                };

                var violations = GetViolationCategories(details);
                var highestScore = GetHighestScore(details);

                return new TextModerationResponseDto
                {
                    IsAppropriate = !flagged,
                    Flagged = flagged,
                    ViolationCategories = violations,
                    Details = details,
                    HighestScore = highestScore,
                    Message = flagged 
                        ? $"⚠️ Contenido inapropiado detectado: {string.Join(", ", violations)}"
                        : "✅ Texto apropiado - no se detectó contenido problemático"
                };
            }
            catch (Exception ex)
            {
                return new TextModerationResponseDto
                {
                    IsAppropriate = false,
                    Flagged = true,
                    Message = $"Error al procesar respuesta: {ex.Message}",
                    ViolationCategories = { "PARSING_ERROR" }
                };
            }
        }

        private bool GetBooleanProperty(JsonElement element, string propertyName)
        {
            return element.TryGetProperty(propertyName, out var prop) && prop.GetBoolean();
        }

        private double GetDoubleProperty(JsonElement element, string propertyName)
        {
            return element.TryGetProperty(propertyName, out var prop) ? prop.GetDouble() : 0.0;
        }

        private List<string> GetViolationCategories(TextModerationDetails details)
        {
            var violations = new List<string>();

            if (details.Sexual) violations.Add("CONTENIDO_SEXUAL");
            if (details.Hate) violations.Add("DISCURSO_DE_ODIO");
            if (details.Harassment) violations.Add("ACOSO");
            if (details.SelfHarm) violations.Add("AUTOLESIONES");
            if (details.SexualMinors) violations.Add("CONTENIDO_SEXUAL_MENORES");
            if (details.HateThreatening) violations.Add("AMENAZAS_DE_ODIO");
            if (details.ViolenceGraphic) violations.Add("VIOLENCIA_GRAFICA");
            if (details.SelfHarmIntent) violations.Add("INTENCION_AUTOLESION");
            if (details.SelfHarmInstructions) violations.Add("INSTRUCCIONES_AUTOLESION");
            if (details.HarassmentThreatening) violations.Add("ACOSO_AMENAZANTE");
            if (details.Violence) violations.Add("VIOLENCIA");

            return violations;
        }

        private double GetHighestScore(TextModerationDetails details)
        {
            var scores = new[]
            {
                details.SexualScore,
                details.HateScore,
                details.HarassmentScore,
                details.SelfHarmScore,
                details.SexualMinorsScore,
                details.HateThreateningScore,
                details.ViolenceGraphicScore,
                details.SelfHarmIntentScore,
                details.SelfHarmInstructionsScore,
                details.HarassmentThreateningScore,
                details.ViolenceScore
            };

            return scores.Max();
        }
    }
}