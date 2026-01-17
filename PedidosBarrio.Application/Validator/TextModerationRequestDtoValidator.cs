using FluentValidation;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Validator
{
    public class TextModerationRequestDtoValidator : AbstractValidator<TextModerationRequestDto>
    {
        public TextModerationRequestDtoValidator()
        {
            RuleFor(x => x.Text)
                .NotEmpty()
                .WithMessage("El texto es requerido")
                .MaximumLength(10000)
                .WithMessage("El texto no puede exceder los 10,000 caracteres");

            RuleFor(x => x.Model)
                .Must(model => string.IsNullOrEmpty(model) || 
                              new[] { "omni-moderation-latest", "text-moderation-stable" }.Contains(model))
                .WithMessage("El modelo debe ser 'omni-moderation-latest' o 'text-moderation-stable'");
        }
    }
}