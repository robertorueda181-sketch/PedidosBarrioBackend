using FluentValidation;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Validator
{
    public class CreateProductoDtoValidator : AbstractValidator<CreateProductoDto>
    {
        public CreateProductoDtoValidator()
        {
            RuleFor(dto => dto.EmpresaID)
                .NotEmpty().WithMessage("El ID de la empresa no puede estar vacío.");

            RuleFor(dto => dto.Nombre)
                .NotEmpty().WithMessage("El nombre del producto no puede estar vacío.")
                .MaximumLength(255).WithMessage("El nombre no puede exceder los 255 caracteres.");

            RuleFor(dto => dto.Descripcion)
                .MaximumLength(1000).WithMessage("La descripción no puede exceder los 1000 caracteres.");
        }
    }
}
