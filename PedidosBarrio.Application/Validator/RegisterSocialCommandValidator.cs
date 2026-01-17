using FluentValidation;
using PedidosBarrio.Application.Commands.RegisterSocial;

namespace PedidosBarrio.Application.Validator
{
    public class RegisterSocialCommandValidator : AbstractValidator<RegisterSocialCommand>
    {
        public RegisterSocialCommandValidator()
        {
            // Validaciones básicas de email
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("El email es requerido")
                .EmailAddress()
                .WithMessage("El email debe tener un formato válido")
                .MaximumLength(255)
                .WithMessage("El email no puede exceder los 255 caracteres");

            // Validaciones de nombre
            RuleFor(x => x.Nombre)
                .NotEmpty()
                .WithMessage("El nombre es requerido")
                .MaximumLength(100)
                .WithMessage("El nombre no puede exceder los 100 caracteres");

            // Validaciones de apellido
            RuleFor(x => x.Apellido)
                .NotEmpty()
                .WithMessage("El apellido es requerido")
                .MaximumLength(100)
                .WithMessage("El apellido no puede exceder los 100 caracteres");

            // Validaciones de nombre de usuario
            RuleFor(x => x.NombreUsuario)
                .MaximumLength(50)
                .WithMessage("El nombre de usuario no puede exceder los 50 caracteres")
                .When(x => !string.IsNullOrEmpty(x.NombreUsuario));

            // Validaciones de nombre de empresa
            RuleFor(x => x.NombreEmpresa)
                .NotEmpty()
                .WithMessage("El nombre de la empresa es requerido")
                .MaximumLength(200)
                .WithMessage("El nombre de la empresa no puede exceder los 200 caracteres");

            // Validaciones de descripción
            RuleFor(x => x.Descripcion)
                .NotEmpty()
                .WithMessage("La descripción es requerida")
                .MaximumLength(1000)
                .WithMessage("La descripción no puede exceder los 1000 caracteres");

            // Validaciones de categoría
            RuleFor(x => x.Categoria)
                .NotEmpty()
                .WithMessage("La categoría es requerida")
                .MaximumLength(100)
                .WithMessage("La categoría no puede exceder los 100 caracteres");

            // Validaciones de teléfono
            RuleFor(x => x.Telefono)
                .NotEmpty()
                .WithMessage("El teléfono es requerido")
                .Matches(@"^[+]?[\d\s\-\(\)]+$")
                .WithMessage("El teléfono debe tener un formato válido")
                .MaximumLength(20)
                .WithMessage("El teléfono no puede exceder los 20 caracteres");

            // Validaciones de dirección
            RuleFor(x => x.Direccion)
                .NotEmpty()
                .WithMessage("La dirección es requerida")
                .MaximumLength(300)
                .WithMessage("La dirección no puede exceder los 300 caracteres");

            // Validaciones de referencia
            RuleFor(x => x.Referencia)
                .MaximumLength(200)
                .WithMessage("La referencia no puede exceder los 200 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Referencia));

            // Validaciones de tipo de empresa
            RuleFor(x => x.TipoEmpresa)
                .GreaterThan((short)0)
                .WithMessage("El tipo de empresa es requerido")
                .LessThanOrEqualTo((short)3)
                .WithMessage("El tipo de empresa debe ser 1 (Negocio), 2 (Servicio) o 3 (Inmueble)");

            // Validaciones para registro sin Google (usuario/contraseña)
            When(x => string.IsNullOrEmpty(x.Provider), () => {
                RuleFor(x => x.Contrasena)
                    .NotEmpty()
                    .WithMessage("La contraseña es requerida para registro sin Google")
                    .MinimumLength(8)
                    .WithMessage("La contraseña debe tener al menos 8 caracteres")
                    .MaximumLength(100)
                    .WithMessage("La contraseña no puede exceder los 100 caracteres")
                    .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)")
                    .WithMessage("La contraseña debe contener al menos una minúscula, una mayúscula y un número");
            });

            // Validaciones para registro con Google
            When(x => !string.IsNullOrEmpty(x.Provider), () => {
                RuleFor(x => x.Provider)
                    .Must(provider => provider.ToLower() == "google")
                    .WithMessage("Solo se soporta el provider 'google'");

                RuleFor(x => x.SocialId)
                    .NotEmpty()
                    .WithMessage("El SocialId es requerido para registro con Google");

                RuleFor(x => x.IdToken)
                    .NotEmpty()
                    .WithMessage("El IdToken es requerido para registro con Google");
            });
        }
    }
}