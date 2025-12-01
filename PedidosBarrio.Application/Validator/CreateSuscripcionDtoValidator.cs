using FluentValidation;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Validator
{
    public class CreateSuscripcionDtoValidator : AbstractValidator<CreateSuscripcionDto>
    {
        public CreateSuscripcionDtoValidator()
        {
            RuleFor(dto => dto.EmpresaID)
                .GreaterThan(0).WithMessage("El ID de la empresa debe ser mayor a 0.");

            RuleFor(dto => dto.Monto)
                .GreaterThan(0).WithMessage("El monto debe ser mayor a 0.")
                .LessThanOrEqualTo(999999.99m).WithMessage("El monto no puede exceder 999999.99");
        }
    }
}
