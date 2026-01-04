using FluentValidation;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Validator
{
    public class CreateInmuebleDtoValidator : AbstractValidator<CreateInmuebleDto>
    {
        public CreateInmuebleDtoValidator()
        {

            RuleFor(dto => dto.TiposID)
                .GreaterThan(0).WithMessage("El ID del tipo debe ser mayor a 0.");

            RuleFor(dto => dto.Precio)
                .GreaterThan(0).WithMessage("El precio debe ser mayor a 0.")
                .LessThanOrEqualTo(9999999.99m).WithMessage("El precio no puede exceder 9999999.99");

            RuleFor(dto => dto.Medidas)
                .NotEmpty().WithMessage("Las medidas son obligatorias.")
                .MaximumLength(100).WithMessage("Las medidas no pueden exceder los 100 caracteres.");

            RuleFor(dto => dto.Ubicacion)
                .NotEmpty().WithMessage("La ubicación es obligatoria.")
                .MaximumLength(500).WithMessage("La ubicación no puede exceder los 500 caracteres.");

            RuleFor(dto => dto.Dormitorios)
                .GreaterThanOrEqualTo(0).WithMessage("El número de dormitorios no puede ser negativo.")
                .LessThanOrEqualTo(50).WithMessage("El número de dormitorios no puede exceder 50.");

            RuleFor(dto => dto.Banos)
                .GreaterThanOrEqualTo(0).WithMessage("El número de baños no puede ser negativo.")
                .LessThanOrEqualTo(50).WithMessage("El número de baños no puede exceder 50.");

            RuleFor(dto => dto.Descripcion)
                .MaximumLength(1000).WithMessage("La descripción no puede exceder los 1000 caracteres.");
        }
    }
}
