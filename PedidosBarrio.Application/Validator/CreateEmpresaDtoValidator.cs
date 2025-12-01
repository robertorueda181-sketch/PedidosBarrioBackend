using FluentValidation;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Validator
{
    public class CreateEmpresaDtoValidator : AbstractValidator<CreateEmpresaDto>
    {
        public CreateEmpresaDtoValidator()
        {
            RuleFor(dto => dto.Nombre)
                .NotEmpty().WithMessage("El nombre de la empresa no puede estar vacío.")
                .MaximumLength(255).WithMessage("El nombre no puede exceder los 255 caracteres.");

            RuleFor(dto => dto.Descripcion)
                .MaximumLength(1000).WithMessage("La descripción no puede exceder los 1000 caracteres.");

            RuleFor(dto => dto.Direccion)
                .NotEmpty().WithMessage("La dirección es obligatoria.")
                .MaximumLength(500).WithMessage("La dirección no puede exceder los 500 caracteres.");

            RuleFor(dto => dto.Referencia)
                .MaximumLength(255).WithMessage("La referencia no puede exceder los 255 caracteres.");

            RuleFor(dto => dto.Email)
                .NotEmpty().WithMessage("El email es obligatorio.")
                .EmailAddress().WithMessage("El email debe ser válido.")
                .MaximumLength(255).WithMessage("El email no puede exceder los 255 caracteres.");

            RuleFor(dto => dto.Contrasena)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
                .Must(ContainsUppercaseAndLowercase).WithMessage("La contraseña debe contener mayúsculas y minúsculas.")
                .Must(ContainsNumber).WithMessage("La contraseña debe contener al menos un número.")
                .Must(ContainsSpecialCharacter).WithMessage("La contraseña debe contener al menos un carácter especial.");

            RuleFor(dto => dto.Telefono)
                .NotEmpty().WithMessage("El teléfono es obligatorio.")
                .Matches(@"^\+?[\d\s\-\(\)]{9,20}$").WithMessage("El teléfono debe ser válido.");
        }

        private bool ContainsUppercaseAndLowercase(string password)
        {
            return password.Any(char.IsUpper) && password.Any(char.IsLower);
        }

        private bool ContainsNumber(string password)
        {
            return password.Any(char.IsDigit);
        }

        private bool ContainsSpecialCharacter(string password)
        {
            return password.Any(c => !char.IsLetterOrDigit(c));
        }
    }
}
