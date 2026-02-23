using FluentValidation;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Validator;

public class CreateClienteDireccionDtoValidator : AbstractValidator<CreateClienteDireccionDto>
{
    public CreateClienteDireccionDtoValidator()
    {
        // Validación del nombre
        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre de la dirección es requerido")
            .MaximumLength(100)
            .WithMessage("El nombre no puede exceder 100 caracteres");

        // Validación del texto de dirección
        RuleFor(x => x.DireccionTexto)
            .NotEmpty()
            .WithMessage("El texto de la dirección es requerido")
            .MaximumLength(500)
            .WithMessage("La dirección no puede exceder 500 caracteres");

        // Validación de referencia (opcional)
        RuleFor(x => x.Referencia)
            .MaximumLength(255)
            .WithMessage("La referencia no puede exceder 255 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Referencia));

        // Validación de latitud
        RuleFor(x => x.Latitud)
            .InclusiveBetween(-90m, 90m)
            .WithMessage("La latitud debe estar entre -90 y 90");

        // Validación de longitud
        RuleFor(x => x.Longitud)
            .InclusiveBetween(-180m, 180m)
            .WithMessage("La longitud debe estar entre -180 y 180");

        // Validación de Departamento
        RuleFor(x => x.Departamento)
            .MaximumLength(100)
            .WithMessage("El departamento no puede exceder 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Departamento));

        // Validación de Provincia
        RuleFor(x => x.Provincia)
            .MaximumLength(100)
            .WithMessage("La provincia no puede exceder 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Provincia));

        // Validación de Distrito
        RuleFor(x => x.Distrito)
            .MaximumLength(100)
            .WithMessage("El distrito no puede exceder 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Distrito));

        // Validación de Código Postal
        RuleFor(x => x.CodigoPostal)
            .MaximumLength(20)
            .WithMessage("El código postal no puede exceder 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.CodigoPostal));
    }
}

public class UpdateClienteDireccionDtoValidator : AbstractValidator<UpdateClienteDireccionDto>
{
    public UpdateClienteDireccionDtoValidator()
    {
        // Validación del nombre (opcional en update)
        RuleFor(x => x.Nombre)
            .MaximumLength(100)
            .WithMessage("El nombre no puede exceder 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Nombre));

        // Validación del texto de dirección (opcional en update)
        RuleFor(x => x.DireccionTexto)
            .MaximumLength(500)
            .WithMessage("La dirección no puede exceder 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.DireccionTexto));

        // Validación de referencia
        RuleFor(x => x.Referencia)
            .MaximumLength(255)
            .WithMessage("La referencia no puede exceder 255 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Referencia));

        // Validación de latitud
        RuleFor(x => x.Latitud)
            .InclusiveBetween(-90m, 90m)
            .WithMessage("La latitud debe estar entre -90 y 90")
            .When(x => x.Latitud.HasValue);

        // Validación de longitud
        RuleFor(x => x.Longitud)
            .InclusiveBetween(-180m, 180m)
            .WithMessage("La longitud debe estar entre -180 y 180")
            .When(x => x.Longitud.HasValue);

        // Validaciones de ubicación administrativa
        RuleFor(x => x.Departamento)
            .MaximumLength(100)
            .WithMessage("El departamento no puede exceder 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Departamento));

        RuleFor(x => x.Provincia)
            .MaximumLength(100)
            .WithMessage("La provincia no puede exceder 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Provincia));

        RuleFor(x => x.Distrito)
            .MaximumLength(100)
            .WithMessage("El distrito no puede exceder 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Distrito));

        RuleFor(x => x.CodigoPostal)
            .MaximumLength(20)
            .WithMessage("El código postal no puede exceder 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.CodigoPostal));
    }
}
