using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Services;
using System.Text.RegularExpressions;

namespace PedidosBarrio.Infrastructure.Services
{
    public class HybridTextModerationService : ITextModerationService
    {
        private readonly ITextModerationService _openAIService;
        private readonly HashSet<string> _spanishBadWords;
        private readonly Dictionary<string, string[]> _spanishBadWordCategories;

        public HybridTextModerationService(CachedTextModerationService openAIService)
        {
            _openAIService = openAIService;
            _spanishBadWords = InitializeSpanishBadWords();
            _spanishBadWordCategories = InitializeBadWordCategories();
        }

        public async Task<TextModerationResponseDto> ModerateTextAsync(string text, string model = "omni-moderation-latest")
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

            // 1. Ejecutar detección local de palabras en español
            var localResult = ModerateSpanishWords(text);
            
            // 2. Ejecutar OpenAI Moderation API
            var openAIResult = await _openAIService.ModerateTextAsync(text, model);

            // 3. Combinar resultados (si cualquiera detecta problema, marcar como inapropiado)
            var combinedResult = CombineResults(localResult, openAIResult, text);

            return combinedResult;
        }

        private TextModerationResponseDto ModerateSpanishWords(string text)
        {
            var normalizedText = NormalizeText(text);
            var detectedWords = new List<string>();
            var categories = new List<string>();

            // Verificar palabras exactas
            foreach (var badWord in _spanishBadWords)
            {
                if (ContainsWord(normalizedText, badWord))
                {
                    detectedWords.Add(badWord);
                    
                    // Buscar categoría de la palabra
                    foreach (var category in _spanishBadWordCategories)
                    {
                        if (category.Value.Contains(badWord))
                        {
                            categories.Add(category.Key);
                            break;
                        }
                    }
                }
            }

            var isFlagged = detectedWords.Any();

            return new TextModerationResponseDto
            {
                IsAppropriate = !isFlagged,
                Flagged = isFlagged,
                ViolationCategories = categories.Distinct().ToList(),
                HighestScore = isFlagged ? 0.95 : 0.05,
                Details = new TextModerationDetails
                {
                    Sexual = categories.Contains("CONTENIDO_SEXUAL_ES"),
                    Hate = categories.Contains("DISCURSO_ODIO_ES"),
                    Harassment = categories.Contains("ACOSO_ES"),
                    Violence = categories.Contains("VIOLENCIA_ES"),
                    SexualScore = categories.Contains("CONTENIDO_SEXUAL_ES") ? 0.95 : 0.05,
                    HateScore = categories.Contains("DISCURSO_ODIO_ES") ? 0.95 : 0.05,
                    HarassmentScore = categories.Contains("ACOSO_ES") ? 0.95 : 0.05,
                    ViolenceScore = categories.Contains("VIOLENCIA_ES") ? 0.95 : 0.05
                },
                Message = isFlagged 
                    ? $"🔍 Filtro español: Palabras inapropiadas detectadas: {string.Join(", ", detectedWords.Take(3))}"
                    : "✅ Filtro español: No se detectaron palabras problemáticas"
            };
        }

        private TextModerationResponseDto CombineResults(
            TextModerationResponseDto localResult, 
            TextModerationResponseDto openAIResult, 
            string originalText)
        {
            // Si cualquiera de los dos detecta problema, marcar como inapropiado
            var isInappropriate = localResult.Flagged || openAIResult.Flagged;
            
            // Combinar categorías de violación
            var combinedViolations = new List<string>();
            combinedViolations.AddRange(localResult.ViolationCategories);
            combinedViolations.AddRange(openAIResult.ViolationCategories);

            // Score más alto de ambos sistemas
            var highestScore = Math.Max(localResult.HighestScore, openAIResult.HighestScore);

            // Combinar detalles
            var combinedDetails = new TextModerationDetails
            {
                Sexual = localResult.Details.Sexual || openAIResult.Details.Sexual,
                Hate = localResult.Details.Hate || openAIResult.Details.Hate,
                Harassment = localResult.Details.Harassment || openAIResult.Details.Harassment,
                Violence = localResult.Details.Violence || openAIResult.Details.Violence,
                SelfHarm = openAIResult.Details.SelfHarm, // Solo OpenAI
                SexualMinors = openAIResult.Details.SexualMinors, // Solo OpenAI
                
                SexualScore = Math.Max(localResult.Details.SexualScore, openAIResult.Details.SexualScore),
                HateScore = Math.Max(localResult.Details.HateScore, openAIResult.Details.HateScore),
                HarassmentScore = Math.Max(localResult.Details.HarassmentScore, openAIResult.Details.HarassmentScore),
                ViolenceScore = Math.Max(localResult.Details.ViolenceScore, openAIResult.Details.ViolenceScore),
                SelfHarmScore = openAIResult.Details.SelfHarmScore,
                SexualMinorsScore = openAIResult.Details.SexualMinorsScore
            };

            var message = "";
            if (isInappropriate)
            {
                var sources = new List<string>();
                if (localResult.Flagged) sources.Add("Filtro Español");
                if (openAIResult.Flagged) sources.Add("OpenAI");
                
                message = $"⚠️ Contenido inapropiado detectado por: {string.Join(" + ", sources)}";
            }
            else
            {
                message = "✅ Texto apropiado según ambos sistemas de moderación";
            }

            return new TextModerationResponseDto
            {
                IsAppropriate = !isInappropriate,
                Flagged = isInappropriate,
                ViolationCategories = combinedViolations.Distinct().ToList(),
                Details = combinedDetails,
                HighestScore = highestScore,
                Message = message
            };
        }

        private string NormalizeText(string text)
        {
            // Convertir a minúsculas y remover acentos
            var normalized = text.ToLowerInvariant();
            
            // Reemplazar caracteres con acentos
            var replacements = new Dictionary<char, char>
            {
                {'á', 'a'}, {'é', 'e'}, {'í', 'i'}, {'ó', 'o'}, {'ú', 'u'}, {'ü', 'u'},
                {'ñ', 'n'}, {'ç', 'c'}
            };

            foreach (var replacement in replacements)
            {
                normalized = normalized.Replace(replacement.Key, replacement.Value);
            }

            return normalized;
        }

        private bool ContainsWord(string text, string word)
        {
            // Usar regex para buscar la palabra completa o como parte de otra palabra
            var patterns = new[]
            {
                $@"\b{Regex.Escape(word)}\b",        // Palabra completa
                $@"{Regex.Escape(word)}",           // Como substring
            };

            return patterns.Any(pattern => Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase));
        }

        private HashSet<string> InitializeSpanishBadWords()
        {
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                // Palabras sexuales
                "puta", "putita", "puto", "putito", "perra", "zorra", "ramera",
                "prostituta", "escort", "prepago", "cachonda", "cachondo", "caliente",
                "porno", "pornografia", "pornero", "xxx", "sexo", "coger", "cochar",
                "cachar", "cachera", "cachero", "follar", "joder", "chingar",
                "mamada", "mamadas", "chupar", "chupada", "verga", "pene", "pito",
                "vagina", "cono", "teta", "tetas", "culo", "nalgas", "trasero",
                
                // Insultos y groserías
                "cabron", "cabrona", "hijo de puta", "hijueputa", "maldito", "maldita",
                "pendejo", "pendeja", "idiota", "estupido", "estupida", "imbecil",
                "gilipollas", "capullo", "mamaguevo", "gonorrea", "marica", "maricon",
                
                // Violencia
                "matar", "asesinar", "violencia", "golpear", "pegar", "romper la cara",
                "partir la madre", "madrazo", "vergazo", "putazo", "golpiza",
                
                // Drogas
                "marihuana", "cocaina", "heroina", "drogas", "drogadicto", "drogadicta",
                "marihuano", "marihuanero", "cocainomano", "adicto",
                
                // Términos de odio
                "negro", "negra", "indio", "india", "cholo", "chola", "naco", "naca",
                "pobreton", "muerto de hambre", "raza", "racista"
            };
        }

        private Dictionary<string, string[]> InitializeBadWordCategories()
        {
            return new Dictionary<string, string[]>
            {
                ["CONTENIDO_SEXUAL_ES"] = new[]
                {
                    "puta", "putita", "puto", "putito", "perra", "zorra", "ramera",
                    "prostituta", "escort", "prepago", "cachonda", "cachondo",
                    "porno", "pornografia", "pornero", "xxx", "sexo", "coger",
                    "cachar", "cachera", "cachero", "follar", "mamada", "verga",
                    "pito", "vagina", "cono", "teta", "tetas", "culo"
                },
                ["ACOSO_ES"] = new[]
                {
                    "cabron", "cabrona", "hijo de puta", "hijueputa", "pendejo",
                    "pendeja", "idiota", "estupido", "estupida", "imbecil",
                    "gilipollas", "mamaguevo"
                },
                ["VIOLENCIA_ES"] = new[]
                {
                    "matar", "asesinar", "violencia", "golpear", "pegar",
                    "romper la cara", "partir la madre", "madrazo", "golpiza"
                },
                ["DISCURSO_ODIO_ES"] = new[]
                {
                    "marica", "maricon", "negro", "negra", "indio", "india",
                    "cholo", "chola", "naco", "naca", "racista"
                }
            };
        }
    }
}