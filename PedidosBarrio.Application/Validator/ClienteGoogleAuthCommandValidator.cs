using FluentValidation;
using PedidosBarrio.Application.Commands.ClienteAuth;

namespace PedidosBarrio.Application.Validator;

public class ClienteGoogleAuthCommandValidator : AbstractValidator<ClienteGoogleAuthCommand>
{
    public ClienteGoogleAuthCommandValidator()
    {
        // Validación del IdToken
        RuleFor(x => x.IdToken)
            .NotEmpty()
            .WithMessage("El ID token de Google es requerido")
            .MinimumLength(10)
            .WithMessage("El ID token tiene un formato inválido");

        // Validación del DNI
        RuleFor(x => x.DNI)
            .NotEmpty()
            .WithMessage("El DNI es requerido")
            .Matches(@"^\d{8}$")
            .WithMessage("El DNI debe contener exactamente 8 dígitos");

        // Validación del teléfono (opcional)
        RuleFor(x => x.Telefono)
            .Matches(@"^\+?[0-9]{7,15}$")
            .WithMessage("El teléfono debe contener entre 7 y 15 dígitos")
            .When(x => !string.IsNullOrEmpty(x.Telefono));

        // Validación de latitud (debe estar entre -90 y 90)
        RuleFor(x => x.Latitud)
            .InclusiveBetween(-90m, 90m)
            .WithMessage("La latitud debe estar entre -90 y 90")
            .When(x => x.Latitud.HasValue);

        // Validación de longitud (debe estar entre -180 y 180)
        RuleFor(x => x.Longitud)
            .InclusiveBetween(-180m, 180m)
            .WithMessage("La longitud debe estar entre -180 y 180")
            .When(x => x.Longitud.HasValue);

        // Validación cruzada: si se proporciona latitud, también debe haber longitud
        RuleFor(x => x.Longitud)
            .NotNull()
            .WithMessage("Si se proporciona latitud, también debe proporcionar longitud")
            .When(x => x.Latitud.HasValue);

        // Validación cruzada: si se proporciona longitud, también debe haber latitud
        RuleFor(x => x.Latitud)
            .NotNull()
            .WithMessage("Si se proporciona longitud, también debe proporcionar latitud")
            .When(x => x.Longitud.HasValue);
    }
}
