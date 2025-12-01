using FluentValidation;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Validator
{
    public class CreateProductoDtoValidator : AbstractValidator<CreateProductoDto>
    {
        public CreateProductoDtoValidator()
        {
            RuleFor(dto => dto.EmpresaID)
                .GreaterThan(0).WithMessage("El ID de la empresa debe ser mayor a 0.");

            RuleFor(dto => dto.Nombre)
                .NotEmpty().WithMessage("El nombre del producto no puede estar vacío.")
                .MaximumLength(255).WithMessage("El nombre no puede exceder los 255 caracteres.");

            RuleFor(dto => dto.Descripcion)
                .MaximumLength(1000).WithMessage("La descripción no puede exceder los 1000 caracteres.");
        }
    }
}
