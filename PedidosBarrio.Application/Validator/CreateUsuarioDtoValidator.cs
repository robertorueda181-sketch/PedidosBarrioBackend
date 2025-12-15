using FluentValidation;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Validator
{
    public class CreateUsuarioDtoValidator : AbstractValidator<CreateUsuarioDto>
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public CreateUsuarioDtoValidator(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;

            RuleFor(dto => dto.NombreUsuario)
                .NotEmpty().WithMessage("El nombre de usuario no puede estar vacío.")
                .MinimumLength(3).WithMessage("El nombre de usuario debe tener al menos 3 caracteres.")
                .MaximumLength(50).WithMessage("El nombre de usuario no puede exceder los 50 caracteres.")
                .MustAsync(NombreUsuarioNotExist).WithMessage("El nombre de usuario ya está registrado.");

            RuleFor(dto => dto.Email)
                .NotEmpty().WithMessage("El email es obligatorio.")
                .EmailAddress().WithMessage("El email debe ser válido.")
                .MaximumLength(255).WithMessage("El email no puede exceder los 255 caracteres.")
                .MustAsync(EmailNotExist).WithMessage("El email ya está registrado en el sistema.");

            RuleFor(dto => dto.Contrasena)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
                .Must(ContainsUppercaseAndLowercase).WithMessage("La contraseña debe contener mayúsculas y minúsculas.")
                .Must(ContainsNumber).WithMessage("La contraseña debe contener al menos un número.")
                .Must(ContainsSpecialCharacter).WithMessage("La contraseña debe contener al menos un carácter especial.");

            RuleFor(dto => dto.EmpresaID)
                .NotEqual(Guid.Empty).WithMessage("El EmpresaID es obligatorio.");
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

        private async Task<bool> NombreUsuarioNotExist(string nombreUsuario, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
                return true;

            try
            {
                var usuario = await _usuarioRepository.GetByNombreUsuarioAsync(nombreUsuario);
                return usuario == null;
            }
            catch
            {
                return true;
            }
        }

        private async Task<bool> EmailNotExist(string email, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(email))
                return true;

            try
            {
                var usuario = await _usuarioRepository.GetByEmailAsync(email);
                return usuario == null;
            }
            catch
            {
                return true;
            }
        }
    }
}
