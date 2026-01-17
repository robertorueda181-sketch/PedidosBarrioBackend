using FluentValidation;
using FluentValidation.Results;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Services;

namespace PedidosBarrio.Application.Commands.ValidateImage
{
    public class ValidateImageCommandHandler : IRequestHandler<ValidateImageCommand, ImageValidationResponseDto>
    {
        private readonly IImageModerationService _imageModerationService;
        private readonly IApplicationLogger _logger;

        public ValidateImageCommandHandler(
            IImageModerationService imageModerationService,
            IApplicationLogger logger)
        {
            _imageModerationService = imageModerationService;
            _logger = logger;
        }

        public async Task<ImageValidationResponseDto> Handle(ValidateImageCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Validar que al menos una fuente de imagen esté presente
                if (string.IsNullOrEmpty(command.ImageUrl) && string.IsNullOrEmpty(command.Base64Image))
                {
                    throw new ValidationException("Debe proporcionar ImageUrl o Base64Image");
                }

                // Log de la operación (sin información de usuario)
                await _logger.LogInformationAsync(
                    "Validación de imagen solicitada (endpoint público)",
                    "ValidateImageCommand");

                ImageValidationResponseDto result;

                // Procesar según el tipo de entrada
                if (!string.IsNullOrEmpty(command.Base64Image))
                {
                    result = await _imageModerationService.ValidateImageFromBase64Async(
                        command.Base64Image, 
                        command.ToleranceLevel ?? "MEDIUM");
                }
                else
                {
                    result = await _imageModerationService.ValidateImageAsync(
                        command.ImageUrl, 
                        command.ToleranceLevel ?? "MEDIUM");
                }

                // Log del resultado
                await _logger.LogInformationAsync(
                    $"Validación completada - Apropiada: {result.IsAppropriate}, Confianza: {result.ConfidenceScore:P}",
                    "ValidateImageCommand");

                return result;
            }
            catch (ValidationException)
            {
                throw; // Re-lanzar excepciones de validación
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(
                    $"Error al validar imagen: {ex.Message}",
                    ex,
                    "ValidateImageCommand");

                return new ImageValidationResponseDto
                {
                    IsAppropriate = false,
                    ConfidenceScore = 0,
                    Message = $"Error interno al procesar la imagen: {ex.Message}",
                    ViolationReasons = { "INTERNAL_ERROR" }
                };
            }
        }
    }
}