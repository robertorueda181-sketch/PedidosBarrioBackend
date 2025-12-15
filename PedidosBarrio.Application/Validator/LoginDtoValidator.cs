using FluentValidation;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Validator
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(dto => dto.Email)
                .NotEmpty().WithMessage("El email es obligatorio.")
                .EmailAddress().WithMessage("El email debe ser válido.");

            RuleFor(dto => dto.Contrasena)
                .NotEmpty().WithMessage("La contraseña es obligatoria.");
        }
    }
}
