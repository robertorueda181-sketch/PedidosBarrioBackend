using FluentValidation;
using PedidosBarrio.Application.Commands.ModerateText;

namespace PedidosBarrio.Application.Validator
{
    public class ModerateTextCommandValidator : AbstractValidator<ModerateTextCommand>
    {
        public ModerateTextCommandValidator()
        {
            RuleFor(x => x.Text)
                .NotEmpty()
                .WithMessage("El texto no puede estar vacío")
                .MaximumLength(10000)
                .WithMessage("El texto no puede exceder los 10,000 caracteres");

            RuleFor(x => x.Model)
                .Must(model => string.IsNullOrEmpty(model) || 
                              new[] { "omni-moderation-latest", "text-moderation-stable" }.Contains(model))
                .WithMessage("El modelo debe ser 'omni-moderation-latest' o 'text-moderation-stable'");
        }
    }
}