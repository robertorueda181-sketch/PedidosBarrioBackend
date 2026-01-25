using FluentValidation;
using FluentValidation.Results;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.ValidateImage
{
    public class ValidateImageCommandHandler : IRequestHandler<ValidateImageCommand, ImageValidationResponseDto>
    {
        private readonly IImageModerationService _imageModerationService;
        private readonly IApplicationLogger _logger;
        private readonly IValidator<ValidateImageCommand> _validator;
        private readonly IIaModeracionLogRepository _iaModeracionLogRepository;
        private readonly ICurrentUserService _currentUserService;

        public ValidateImageCommandHandler(
            IImageModerationService imageModerationService,
            IApplicationLogger logger,
            IValidator<ValidateImageCommand> validator,
            IIaModeracionLogRepository iaModeracionLogRepository,
            ICurrentUserService currentUserService)
        {
            _imageModerationService = imageModerationService;
            _logger = logger;
            _validator = validator;
            _iaModeracionLogRepository = iaModeracionLogRepository;
            _currentUserService = currentUserService;
        }

        public async Task<ImageValidationResponseDto> Handle(ValidateImageCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // ===== VALIDAR ENTRADA CON FLUENTVALIDATION =====
                var validationResult = await _validator.ValidateAsync(command, cancellationToken);
                if (!validationResult.IsValid)
                {
                    throw new ValidationException(validationResult.Errors);
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

                // ===== GUARDAR LOG DE IA SI HAY UNA EMPRESA ASOCIADA =====
                try
                {
                    var empresaId = _currentUserService.GetEmpresaId();
                    if (empresaId != Guid.Empty)
                    {
                        var iaLog = new IaModeracionLog
                        {
                                EmpresaID = empresaId,
                                EsTexto = false,
                                Apropiado = result.IsAppropriate,
                                Evaluacion = result.IsAppropriate ? $"Apropiada (Confianza: {result.ConfidenceScore:P})" : $"Marcada: {string.Join(", ", result.ViolationReasons)}",
                                Contexto = command.ImageUrl ?? "Imagen en Base64",
                                FechaRegistro = DateTime.UtcNow
                            };
                        await _iaModeracionLogRepository.AddAsync(iaLog);
                    }
                }
                catch (Exception ex)
                {
                    await _logger.LogWarningAsync($"Error al guardar log de IA en base de datos: {ex.Message}", "ValidateImageCommand");
                }

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