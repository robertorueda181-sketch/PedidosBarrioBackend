using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Services;

namespace PedidosBarrio.Infrastructure.Services
{
    /// <summary>
    /// Servicio de moderación MOCK para testing sin Google Vision API
    /// </summary>
    public class MockImageModerationService : IImageModerationService
    {
        public async Task<ImageValidationResponseDto> ValidateImageAsync(string imageUrl, string toleranceLevel = "MEDIUM")
        {
            // Simular análisis de la imagen
            await Task.Delay(1000); // Simular tiempo de procesamiento

            // Para testing, considera inapropiadas las URLs que contengan ciertas palabras
            var inappropriateKeywords = new[] { "adult", "violence", "inappropriate", "bad", "nsfw" };
            var isInappropriate = inappropriateKeywords.Any(keyword => 
                imageUrl.ToLower().Contains(keyword));

            return new ImageValidationResponseDto
            {
                IsAppropriate = !isInappropriate,
                ConfidenceScore = 0.85,
                ViolationReasons = isInappropriate 
                    ? new List<string> { "CONTENIDO_DETECTADO_EN_URL" }
                    : new List<string>(),
                Details = new ImageModerationDetails
                {
                    HasAdultContent = isInappropriate,
                    HasViolentContent = false,
                    HasRacyContent = false,
                    AdultLikelihood = isInappropriate ? "LIKELY" : "VERY_UNLIKELY",
                    ViolenceLikelihood = "VERY_UNLIKELY",
                    RacyLikelihood = "VERY_UNLIKELY"
                },
                Message = isInappropriate 
                    ? "⚠️ MOCK: Imagen detectada como inapropiada (basado en URL)"
                    : "✅ MOCK: Imagen apropiada (simulación sin Google Vision)"
            };
        }

        public async Task<ImageValidationResponseDto> ValidateImageFromBase64Async(string base64Image, string toleranceLevel = "MEDIUM")
        {
            await Task.Delay(1500); // Simular tiempo de procesamiento

            // Para Base64, hacer una validación básica
            var isValidBase64 = IsValidBase64(base64Image);

            return new ImageValidationResponseDto
            {
                IsAppropriate = isValidBase64,
                ConfidenceScore = 0.90,
                ViolationReasons = isValidBase64 
                    ? new List<string>() 
                    : new List<string> { "FORMATO_INVALIDO" },
                Details = new ImageModerationDetails
                {
                    HasAdultContent = false,
                    HasViolentContent = false,
                    HasRacyContent = false,
                    AdultLikelihood = "VERY_UNLIKELY",
                    ViolenceLikelihood = "VERY_UNLIKELY",
                    RacyLikelihood = "VERY_UNLIKELY"
                },
                Message = isValidBase64
                    ? "✅ MOCK: Imagen Base64 válida (simulación sin Google Vision)"
                    : "❌ MOCK: Formato Base64 inválido"
            };
        }

        private bool IsValidBase64(string base64)
        {
            try
            {
                var bytes = Convert.FromBase64String(base64);
                return bytes.Length > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}