using FluentValidation;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Validators;

public class CreatePaginaDtoValidator : AbstractValidator<CreatePaginaDto>
{
    public CreatePaginaDtoValidator()
    {
        RuleFor(x => x.Contenido)
            .NotEmpty().WithMessage("Contenido es requerido")
            .Must(BeValidJson).WithMessage("Contenido debe ser JSON válido");

        RuleFor(x => x.Descripcion)
            .MaximumLength(500).WithMessage("Descripción no puede exceder 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descripcion));
    }

    private static bool BeValidJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return false;

        try
        {
            System.Text.Json.JsonDocument.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

public class UpdatePaginaDtoValidator : AbstractValidator<UpdatePaginaDto>
{
    public UpdatePaginaDtoValidator()
    {
        RuleFor(x => x.Contenido)
            .Must(BeValidJson).WithMessage("Contenido debe ser JSON válido")
            .When(x => !string.IsNullOrEmpty(x.Contenido));

        RuleFor(x => x.Descripcion)
            .MaximumLength(500).WithMessage("Descripción no puede exceder 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descripcion));
    }

    private static bool BeValidJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return false;

        try
        {
            System.Text.Json.JsonDocument.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
