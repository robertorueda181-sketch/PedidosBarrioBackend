using FluentValidation;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.ModerateText
{
    public class ModerateTextCommandHandler : IRequestHandler<ModerateTextCommand, TextModerationResponseDto>
    {
        private readonly ITextModerationService _textModerationService;
        private readonly IApplicationLogger _logger;
        private readonly IValidator<ModerateTextCommand> _validator;
        private readonly IIaModeracionLogRepository _iaModeracionLogRepository;
        private readonly ICurrentUserService _currentUserService;

        public ModerateTextCommandHandler(
            ITextModerationService textModerationService,
            IApplicationLogger logger,
            IValidator<ModerateTextCommand> validator,
            IIaModeracionLogRepository iaModeracionLogRepository,
            ICurrentUserService currentUserService)
        {
            _textModerationService = textModerationService;
            _logger = logger;
            _validator = validator;
            _iaModeracionLogRepository = iaModeracionLogRepository;
            _currentUserService = currentUserService;
        }

        public async Task<TextModerationResponseDto> Handle(ModerateTextCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // ===== VALIDAR ENTRADA CON FLUENTVALIDATION =====
                var validationResult = await _validator.ValidateAsync(command, cancellationToken);
                if (!validationResult.IsValid)
                {
                    throw new ValidationException(validationResult.Errors);
                }

                // Log de la operación
                await _logger.LogInformationAsync(
                    $"Moderación de texto solicitada - Longitud: {command.Text.Length} caracteres",
                    "ModerateTextCommand");

                var result = await _textModerationService.ModerateTextAsync(
                    command.Text,
                    command.Model ?? "omni-moderation-latest");

                // Log del resultado
                await _logger.LogInformationAsync(
                    $"Moderación completada - Apropiado: {result.IsAppropriate}, Flagged: {result.Flagged}, Score: {result.HighestScore:F3}",
                    "ModerateTextCommand");

                // ===== GUARDAR LOG DE IA SI HAY UNA EMPRESA ASOCIADA =====
                try
                {
                    var empresaId = _currentUserService.GetEmpresaId();
                    if (empresaId != Guid.Empty)
                    {
                        var iaLog = new IaModeracionLog
                        {
                                EmpresaID = empresaId,
                                EsTexto = true,
                                Apropiado = result.IsAppropriate,
                                Evaluacion = result.IsAppropriate ? "Contenido apropiado" : $"Marcado: {string.Join(", ", result.ViolationCategories)}",
                                Contexto = command.Text,
                                FechaRegistro = DateTime.UtcNow
                            };
                        await _iaModeracionLogRepository.AddAsync(iaLog);
                    }
                }
                catch (Exception ex)
                {
                    await _logger.LogWarningAsync($"Error al guardar log de IA en base de datos: {ex.Message}", "ModerateTextCommand");
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
                    $"Error al moderar texto: {ex.Message}",
                    ex,
                    "ModerateTextCommand");

                return new TextModerationResponseDto
                {
                    IsAppropriate = false,
                    Flagged = true,
                    Message = $"Error interno al procesar el texto: {ex.Message}",
                    ViolationCategories = { "INTERNAL_ERROR" }
                };
            }
        }
    }
}