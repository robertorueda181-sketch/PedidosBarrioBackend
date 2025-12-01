using FluentValidation;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Validator
{
    public class CreateImagenDtoValidator : AbstractValidator<CreateImagenDto>
    {
        public CreateImagenDtoValidator()
        {
            RuleFor(dto => dto.ProductoID)
                .GreaterThan(0).WithMessage("El ID del producto debe ser mayor a 0.");

            RuleFor(dto => dto.URLImagen)
                .NotEmpty().WithMessage("La URL de la imagen es obligatoria.")
                .Must(BeValidUrl).WithMessage("La URL debe ser válida.")
                .MaximumLength(500).WithMessage("La URL no puede exceder los 500 caracteres.");

            RuleFor(dto => dto.Descripcion)
                .MaximumLength(255).WithMessage("La descripción no puede exceder los 255 caracteres.");
        }

        private bool BeValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}
