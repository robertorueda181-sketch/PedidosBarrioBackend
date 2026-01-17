using Google.Cloud.Vision.V1;
using Microsoft.Extensions.Configuration;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Services;

namespace PedidosBarrio.Infrastructure.Services
{
    public class GoogleVisionImageModerationService : IImageModerationService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ImageAnnotatorClient _visionClient;

        public GoogleVisionImageModerationService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            
            try
            {
                // Configurar las credenciales
                var serviceAccountKeyPath = _configuration.GetSection("GoogleVision:ServiceAccountKeyPath").Value;
                
                if (!string.IsNullOrEmpty(serviceAccountKeyPath) && File.Exists(serviceAccountKeyPath))
                {
                    // Usar service account key file
                    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", serviceAccountKeyPath);
                    _visionClient = ImageAnnotatorClient.Create();
                }
                else
                {
                    // Intentar usar credenciales por defecto (útil para desarrollo local con gcloud auth)
                    _visionClient = ImageAnnotatorClient.Create();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al inicializar Google Vision Client: {ex.Message}. " +
                    $"Asegúrate de configurar GoogleVision:ServiceAccountKeyPath en appsettings.json " +
                    $"o ejecutar 'gcloud auth application-default login'", ex);
            }
        }

        public async Task<ImageValidationResponseDto> ValidateImageAsync(string imageUrl, string toleranceLevel = "MEDIUM")
        {
            try
            {
                // Descargar la imagen desde la URL
                var imageBytes = await DownloadImageAsync(imageUrl);
                return await ValidateImageFromBytesAsync(imageBytes, toleranceLevel);
            }
            catch (Exception ex)
            {
                return new ImageValidationResponseDto
                {
                    IsAppropriate = false,
                    ConfidenceScore = 0,
                    Message = $"Error al procesar la imagen: {ex.Message}",
                    ViolationReasons = { "ERROR_PROCESSING_IMAGE" }
                };
            }
        }

        public async Task<ImageValidationResponseDto> ValidateImageFromBase64Async(string base64Image, string toleranceLevel = "MEDIUM")
        {
            try
            {
                var imageBytes = Convert.FromBase64String(base64Image);
                return await ValidateImageFromBytesAsync(imageBytes, toleranceLevel);
            }
            catch (Exception ex)
            {
                return new ImageValidationResponseDto
                {
                    IsAppropriate = false,
                    ConfidenceScore = 0,
                    Message = $"Error al procesar imagen Base64: {ex.Message}",
                    ViolationReasons = { "ERROR_PROCESSING_IMAGE" }
                };
            }
        }

        private async Task<ImageValidationResponseDto> ValidateImageFromBytesAsync(byte[] imageBytes, string toleranceLevel)
        {
            try
            {
                // Crear la imagen para Google Vision
                var image = Image.FromBytes(imageBytes);

                // Detectar contenido de seguridad
                var response = await _visionClient.DetectSafeSearchAsync(image);

                if (response == null)
                {
                    return new ImageValidationResponseDto
                    {
                        IsAppropriate = true,
                        ConfidenceScore = 0.5,
                        Message = "No se pudo analizar la imagen"
                    };
                }

                return ParseGoogleVisionResponse(response, toleranceLevel);
            }
            catch (Exception ex)
            {
                return new ImageValidationResponseDto
                {
                    IsAppropriate = false,
                    ConfidenceScore = 0,
                    Message = $"Error en Google Vision API: {ex.Message}",
                    ViolationReasons = { "VISION_API_ERROR" }
                };
            }
        }

        private async Task<byte[]> DownloadImageAsync(string imageUrl)
        {
            var response = await _httpClient.GetAsync(imageUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }

        private ImageValidationResponseDto ParseGoogleVisionResponse(SafeSearchAnnotation safeSearch, string toleranceLevel)
        {
            try
            {
                var details = new ImageModerationDetails
                {
                    AdultLikelihood = safeSearch.Adult.ToString(),
                    ViolenceLikelihood = safeSearch.Violence.ToString(),
                    MedicalLikelihood = safeSearch.Medical.ToString(),
                    SpoofLikelihood = safeSearch.Spoof.ToString(),
                    RacyLikelihood = safeSearch.Racy.ToString()
                };

                // Determinar si es apropiada basado en el nivel de tolerancia
                var violations = new List<string>();
                var isAppropriate = true;

                details.HasAdultContent = IsHighRisk(safeSearch.Adult, toleranceLevel);
                details.HasViolentContent = IsHighRisk(safeSearch.Violence, toleranceLevel);
                details.HasMedicalContent = IsHighRisk(safeSearch.Medical, toleranceLevel);
                details.HasSpoofContent = IsHighRisk(safeSearch.Spoof, toleranceLevel);
                details.HasRacyContent = IsHighRisk(safeSearch.Racy, toleranceLevel);

                if (details.HasAdultContent)
                {
                    violations.Add("CONTENIDO_ADULTO");
                    isAppropriate = false;
                }

                if (details.HasViolentContent)
                {
                    violations.Add("CONTENIDO_VIOLENTO");
                    isAppropriate = false;
                }

                if (details.HasRacyContent)
                {
                    violations.Add("CONTENIDO_SUGERENTE");
                    isAppropriate = false;
                }

                // Calcular score de confianza
                var confidenceScore = CalculateConfidenceScore(safeSearch);

                return new ImageValidationResponseDto
                {
                    IsAppropriate = isAppropriate,
                    ConfidenceScore = confidenceScore,
                    ViolationReasons = violations,
                    Details = details,
                    Message = isAppropriate 
                        ? "Imagen apropiada para mostrar" 
                        : $"Imagen inapropiada detectada: {string.Join(", ", violations)}"
                };
            }
            catch (Exception ex)
            {
                return new ImageValidationResponseDto
                {
                    IsAppropriate = false,
                    ConfidenceScore = 0,
                    Message = $"Error al procesar respuesta: {ex.Message}",
                    ViolationReasons = { "PARSING_ERROR" }
                };
            }
        }

        private bool IsHighRisk(Likelihood likelihood, string toleranceLevel)
        {
            var riskLevels = new Dictionary<Likelihood, int>
            {
                { Likelihood.VeryUnlikely, 0 },
                { Likelihood.Unlikely, 1 },
                { Likelihood.Possible, 2 },
                { Likelihood.Likely, 3 },
                { Likelihood.VeryLikely, 4 },
                { Likelihood.Unknown, 1 }
            };

            var thresholds = new Dictionary<string, int>
            {
                { "HIGH", 4 },    // Solo VERY_LIKELY
                { "MEDIUM", 3 },  // LIKELY y VERY_LIKELY
                { "LOW", 2 }      // POSSIBLE, LIKELY y VERY_LIKELY
            };

            var riskLevel = riskLevels.GetValueOrDefault(likelihood, 1);
            var threshold = thresholds.GetValueOrDefault(toleranceLevel, 3);

            return riskLevel >= threshold;
        }

        private double CalculateConfidenceScore(SafeSearchAnnotation safeSearch)
        {
            var scores = new List<double>();

            scores.Add(GetLikelihoodScore(safeSearch.Adult));
            scores.Add(GetLikelihoodScore(safeSearch.Violence));
            scores.Add(GetLikelihoodScore(safeSearch.Medical));
            scores.Add(GetLikelihoodScore(safeSearch.Spoof));
            scores.Add(GetLikelihoodScore(safeSearch.Racy));

            return scores.Any() ? scores.Average() : 0.5;
        }

        private double GetLikelihoodScore(Likelihood likelihood)
        {
            return likelihood switch
            {
                Likelihood.VeryUnlikely => 0.1,
                Likelihood.Unlikely => 0.3,
                Likelihood.Possible => 0.5,
                Likelihood.Likely => 0.7,
                Likelihood.VeryLikely => 0.9,
                _ => 0.5
            };
        }
    }
}