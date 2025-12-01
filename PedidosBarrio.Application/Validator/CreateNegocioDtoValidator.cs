using FluentValidation;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Validator
{
    public class CreateNegocioDtoValidator : AbstractValidator<CreateNegocioDto>
    {
        public CreateNegocioDtoValidator()
        {
            RuleFor(dto => dto.EmpresaID)
                .GreaterThan(0).WithMessage("El ID de la empresa debe ser mayor a 0.");

            RuleFor(dto => dto.TiposID)
                .GreaterThan(0).WithMessage("El ID del tipo debe ser mayor a 0.");

            RuleFor(dto => dto.URLNegocio)
                .NotEmpty().WithMessage("La URL del negocio es obligatoria.")
                .Must(BeValidUrl).WithMessage("La URL debe ser válida.")
                .MaximumLength(500).WithMessage("La URL no puede exceder los 500 caracteres.");

            RuleFor(dto => dto.URLOpcional)
                .Must(BeValidUrlOrEmpty).WithMessage("La URL opcional debe ser válida.")
                .MaximumLength(500).WithMessage("La URL opcional no puede exceder los 500 caracteres.");

            RuleFor(dto => dto.Descripcion)
                .MaximumLength(1000).WithMessage("La descripción no puede exceder los 1000 caracteres.");
        }

        private bool BeValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }

        private bool BeValidUrlOrEmpty(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return true;
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}
