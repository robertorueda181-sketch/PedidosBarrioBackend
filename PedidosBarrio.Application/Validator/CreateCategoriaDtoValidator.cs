using FluentValidation;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Validator
{
    public class CreateCategoriaDtoValidator : AbstractValidator<CreateCategoriaDto>
    {
        public CreateCategoriaDtoValidator()
        {
            RuleFor(x => x.Descripcion)
                .NotEmpty()
                .WithMessage("La descripción es requerida.")
                .MaximumLength(100)
                .WithMessage("La descripción no puede exceder los 100 caracteres.");

            RuleFor(x => x.Color)
                .NotEmpty()
                .WithMessage("El color es requerido.")
                .Matches(@"^#(?:[0-9a-fA-F]{3}){1,2}$")
                .WithMessage("El color debe ser un código hexadecimal válido (ej: #FF0000).");
        }
    }
}