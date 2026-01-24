using FluentValidation;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Validator
{
    public class RegisterSocialRequestValidator : AbstractValidator<RegisterSocialRequestDto>
    {
        public RegisterSocialRequestValidator()
        {
            // DATOS DE USUARIO
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email requerido")
                .EmailAddress().WithMessage("Email inválido");

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("Nombre requerido")
                .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

            RuleFor(x => x.Apellido)
                .MaximumLength(100).WithMessage("El apellido no puede exceder 100 caracteres");

            // Validar nombre de usuario solo si no es Google
            When(x => string.IsNullOrEmpty(x.Provider), () =>
            {


                RuleFor(x => x.Contrasena)
                    .NotEmpty().WithMessage("Contraseña requerida para registro sin Google")
                    .MinimumLength(6).WithMessage("Contraseña debe tener al menos 6 caracteres")
                    .MaximumLength(100).WithMessage("Contraseña no puede exceder 100 caracteres");
            });

            // DATOS DE EMPRESA
            RuleFor(x => x.NombreEmpresa)
                .NotEmpty().WithMessage("Nombre de empresa requerido")
                .MaximumLength(255).WithMessage("Nombre de empresa no puede exceder 255 caracteres");

            RuleFor(x => x.TipoEmpresa)
                .NotEmpty().WithMessage("Tipo de empresa requerido")
                .Must(x => x == 1 || x == 2 || x == 3)
                .WithMessage("Tipo de empresa debe ser 1 (Negocio), 2 (Servicio) o 3 (Inmueble)");

            // Validar categoría (excepto para INMUEBLE - tipo 3)
            When(x => x.TipoEmpresa != 3, () =>
            {
                RuleFor(x => x.Categoria)
                    .NotEmpty().WithMessage("Categoría requerida para Negocio (1) y Servicio (2)")
                    .MaximumLength(100).WithMessage("Categoría no puede exceder 100 caracteres");
            });

            RuleFor(x => x.Telefono)
                .Matches(@"^\+?[0-9]{7,15}$").WithMessage("Teléfono inválido (7-15 dígitos, puede incluir +)")
                .When(x => !string.IsNullOrEmpty(x.Telefono));

            RuleFor(x => x.Descripcion)
                .MaximumLength(500).WithMessage("Descripción no puede exceder 500 caracteres");

            RuleFor(x => x.Direccion)
                .MaximumLength(255).WithMessage("Dirección no puede exceder 255 caracteres");

            RuleFor(x => x.Referencia)
                .MaximumLength(255).WithMessage("Referencia no puede exceder 255 caracteres");

            // LOGIN SOCIAL
            When(x => !string.IsNullOrEmpty(x.Provider), () =>
            {
                RuleFor(x => x.Provider)
                    .Must(x => x == "google").WithMessage("Provider debe ser 'google'");

                RuleFor(x => x.SocialId)
                    .NotEmpty().WithMessage("SocialId requerido para login con Google");

                RuleFor(x => x.IdToken)
                    .NotEmpty().WithMessage("IdToken requerido para login con Google");
            });
        }
    }
}
