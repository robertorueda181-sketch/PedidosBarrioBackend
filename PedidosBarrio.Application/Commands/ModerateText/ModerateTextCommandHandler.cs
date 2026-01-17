using FluentValidation;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Services;

namespace PedidosBarrio.Application.Commands.ModerateText
{
    public class ModerateTextCommandHandler : IRequestHandler<ModerateTextCommand, TextModerationResponseDto>
    {
        private readonly ITextModerationService _textModerationService;
        private readonly IApplicationLogger _logger;

        public ModerateTextCommandHandler(
            ITextModerationService textModerationService,
            IApplicationLogger logger)
        {
            _textModerationService = textModerationService;
            _logger = logger;
        }

        public async Task<TextModerationResponseDto> Handle(ModerateTextCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Validar entrada
                if (string.IsNullOrWhiteSpace(command.Text))
                {
                    throw new ValidationException("El texto no puede estar vacío");
                }

                if (command.Text.Length > 10000)
                {
                    throw new ValidationException("El texto no puede exceder los 10,000 caracteres");
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