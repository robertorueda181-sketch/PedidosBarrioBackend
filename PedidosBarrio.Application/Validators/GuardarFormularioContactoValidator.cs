using FluentValidation;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Validators;

public class GuardarFormularioContactoValidator : AbstractValidator<CreateFormularioContactoDto>
{
    public GuardarFormularioContactoValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres")
            .MinimumLength(2).WithMessage("El nombre debe tener al menos 2 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("El email no es válido")
            .MaximumLength(150).WithMessage("El email no puede exceder los 150 caracteres");

        RuleFor(x => x.Telefono)
            .MaximumLength(20).WithMessage("El teléfono no puede exceder los 20 caracteres")
            .Matches(@"^[+]?[(]?[0-9]{1,4}[)]?[-\s.]?[(]?[0-9]{1,4}[)]?[-\s.]?[0-9]{1,9}$")
            .When(x => !string.IsNullOrWhiteSpace(x.Telefono))
            .WithMessage("El formato del teléfono no es válido");

        RuleFor(x => x.FechaReserva)
            .NotEmpty().WithMessage("La fecha de reserva es requerida")
            .Must(date => date >= DateTime.Today).WithMessage("La fecha de reserva no puede ser en el pasado");

        RuleFor(x => x.HoraReserva)
            .Must(time => time.HasValue == false || time.Value >= TimeSpan.FromHours(8) && time.Value <= TimeSpan.FromHours(22))
            .WithMessage("La hora de reserva debe estar entre 8:00 y 22:00");

        RuleFor(x => x.NumeroPersonas)
            .GreaterThanOrEqualTo(1).WithMessage("El número de personas debe ser al menos 1")
            .LessThanOrEqualTo(50).WithMessage("El número de personas no puede exceder 50");

        RuleFor(x => x.Ocasion)
            .MaximumLength(255).WithMessage("La ocasión no puede exceder los 255 caracteres")
            .When(x => !string.IsNullOrWhiteSpace(x.Ocasion));
    }
}
