using FluentValidation;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Validator
{
    /// <summary>
    /// Validador para RegisterDto
    /// Valida todos los campos requeridos del registro
    /// </summary>
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            // Datos del propietario
            RuleFor(x => x.Fullname)
                .NotEmpty().WithMessage("El nombre completo es requerido")
                .MinimumLength(3).WithMessage("El nombre completo debe tener al menos 3 caracteres")
                .MaximumLength(200).WithMessage("El nombre completo no puede exceder 200 caracteres");

            RuleFor(x => x.DNI)
                .NotEmpty().WithMessage("El DNI es requerido")
                .Matches(@"^\d{8}$").WithMessage("El DNI debe tener 8 dígitos");

            // Datos del negocio
            RuleFor(x => x.BusinessName)
                .NotEmpty().WithMessage("El nombre del negocio es requerido")
                .MinimumLength(3).WithMessage("El nombre del negocio debe tener al menos 3 caracteres")
                .MaximumLength(200).WithMessage("El nombre del negocio no puede exceder 200 caracteres");

            RuleFor(x => x.RUC)
                .NotEmpty().WithMessage("El RUC es requerido")
                .Matches(@"^\d{11}$").WithMessage("El RUC debe tener 11 dígitos");

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("La categoría es requerida")
                .Must(category => IsValidCategory(category))
                .WithMessage("Categoría inválida. Debe ser: Residencial, Comercial, o Servicios");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres");

            // Ubicación
            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("La dirección es requerida")
                .MinimumLength(5).WithMessage("La dirección debe tener al menos 5 caracteres")
                .MaximumLength(255).WithMessage("La dirección no puede exceder 255 caracteres");

            RuleFor(x => x.Latitude)
                .GreaterThanOrEqualTo(-90).WithMessage("Latitud inválida")
                .LessThanOrEqualTo(90).WithMessage("Latitud inválida")
                .When(x => x.UseMap && x.Latitude.HasValue);

            RuleFor(x => x.Longitude)
                .GreaterThanOrEqualTo(-180).WithMessage("Longitud inválida")
                .LessThanOrEqualTo(180).WithMessage("Longitud inválida")
                .When(x => x.UseMap && x.Longitude.HasValue);

            RuleFor(x => x.Reference)
                .MaximumLength(255).WithMessage("La referencia no puede exceder 255 caracteres");

            // Contacto
            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("El teléfono es requerido")
                .Matches(@"^\d{7,15}$").WithMessage("El teléfono debe tener entre 7 y 15 dígitos");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es requerido")
                .EmailAddress().WithMessage("El email debe ser válido")
                .MaximumLength(255).WithMessage("El email no puede exceder 255 caracteres");

            // Credenciales
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("El usuario es requerido")
                .MinimumLength(3).WithMessage("El usuario debe tener al menos 3 caracteres")
                .MaximumLength(100).WithMessage("El usuario no puede exceder 100 caracteres")
                .Matches(@"^[a-zA-Z0-9_-]+$").WithMessage("El usuario solo puede contener letras, números, guiones y guiones bajos");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es requerida")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres")
                .MaximumLength(255).WithMessage("La contraseña no puede exceder 255 caracteres")
                .Matches(@"[A-Z]").WithMessage("La contraseña debe contener al menos una mayúscula")
                .Matches(@"[a-z]").WithMessage("La contraseña debe contener al menos una minúscula")
                .Matches(@"[0-9]").WithMessage("La contraseña debe contener al menos un número")
                .Matches(@"[!@#$%^&*()_\-+=\[\]{};':""\\|,.<>\/?]").WithMessage("La contraseña debe contener al menos un carácter especial");

            // Horarios (opcional pero si existen deben ser válidos)
            RuleForEach(x => x.Schedules)
                .ChildRules(schedule =>
                {
                    schedule.RuleFor(s => s.Day)
                        .NotEmpty().WithMessage("El día del horario no puede estar vacío")
                        .Must(day => IsValidDay(day))
                        .WithMessage("El día debe ser: Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday");

                    schedule.RuleFor(s => s.OpenTime)
                        .NotEmpty().WithMessage("La hora de apertura no puede estar vacía")
                        .Matches(@"^\d{2}:\d{2}$").WithMessage("La hora debe estar en formato HH:mm")
                        .When(s => !s.IsClosed);

                    schedule.RuleFor(s => s.CloseTime)
                        .NotEmpty().WithMessage("La hora de cierre no puede estar vacía")
                        .Matches(@"^\d{2}:\d{2}$").WithMessage("La hora debe estar en formato HH:mm")
                        .When(s => !s.IsClosed);
                });
        }

        private bool IsValidCategory(string? category)
        {
            if (string.IsNullOrEmpty(category))
                return false;

            var validCategories = new[] { "Residencial", "Comercial", "Servicios", "Casa", "Apartamento", "Oficina", "Tienda", "Negocio" };
            return validCategories.Contains(category, StringComparer.OrdinalIgnoreCase);
        }

        private bool IsValidDay(string? day)
        {
            if (string.IsNullOrEmpty(day))
                return false;

            var validDays = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };
            return validDays.Contains(day, StringComparer.OrdinalIgnoreCase);
        }
    }
}
