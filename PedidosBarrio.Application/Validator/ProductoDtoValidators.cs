using FluentValidation;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Validator
{
    public class CreateProductoDtoValidator : AbstractValidator<CreateProductoDto>
    {
        public CreateProductoDtoValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty()
                .WithMessage("El nombre del producto es requerido")
                .MaximumLength(200)
                .WithMessage("El nombre no puede exceder los 200 caracteres");

            RuleFor(x => x.Descripcion)
                .NotEmpty()
                .WithMessage("La descripción es requerida")
                .MaximumLength(1000)
                .WithMessage("La descripción no puede exceder los 1000 caracteres");

            RuleFor(x => x.CategoriaID)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar una categoría válida");

            RuleFor(x => x.Precio)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El precio debe ser mayor o igual a 0");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El stock debe ser mayor o igual a 0");

            RuleFor(x => x.Imagen)
                .MaximumLength(500)
                .WithMessage("La URL de la imagen no puede exceder los 500 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Imagen));
        }
    }

    public class UpdateProductoDtoValidator : AbstractValidator<UpdateProductoDto>
    {
        public UpdateProductoDtoValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty()
                .WithMessage("El nombre del producto es requerido")
                .MaximumLength(200)
                .WithMessage("El nombre no puede exceder los 200 caracteres");

            RuleFor(x => x.Descripcion)
                .NotEmpty()
                .WithMessage("La descripción es requerida")
                .MaximumLength(1000)
                .WithMessage("La descripción no puede exceder los 1000 caracteres");

            RuleFor(x => x.CategoriaID)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar una categoría válida");

            RuleFor(x => x.Precio)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El precio debe ser mayor o igual a 0");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El stock debe ser mayor o igual a 0");

            RuleFor(x => x.Imagen)
                .MaximumLength(500)
                .WithMessage("La URL de la imagen no puede exceder los 500 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Imagen));
        }
    }
}