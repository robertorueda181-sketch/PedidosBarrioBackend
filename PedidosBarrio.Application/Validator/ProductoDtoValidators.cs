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
                .MaximumLength(1000)
                .WithMessage("La descripción no puede exceder los 1000 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Descripcion));

                    RuleFor(x => x.CategoriaID)
                        .GreaterThan((short)0)
                        .WithMessage("Debe seleccionar una categoría válida");

                    RuleFor(x => x.Precios)
                        .NotEmpty()
                        .WithMessage("Debe proporcionar al menos un precio")
                        .Must(precios => precios.Count <= 10)
                        .WithMessage("No puede tener más de 10 precios por producto");

                    RuleForEach(x => x.Precios).SetValidator(new PrecioCreateDtoValidator());

                    RuleFor(x => x.Stock)
                        .GreaterThanOrEqualTo(0)
                        .WithMessage("El stock debe ser mayor o igual a 0");

                    RuleFor(x => x.StockMinimo)
                        .GreaterThanOrEqualTo(0)
                        .WithMessage("El stock mínimo debe ser mayor o igual a 0")
                        .When(x => x.StockMinimo.HasValue);
                }
            }

            public class PrecioCreateDtoValidator : AbstractValidator<PrecioCreateDto>
            {
                public PrecioCreateDtoValidator()
                {
                    RuleFor(x => x.Precio)
                        .GreaterThan(0)
                        .WithMessage("El precio debe ser mayor a 0");

                    RuleFor(x => x.Descripcion)
                        .MaximumLength(200)
                        .WithMessage("La descripción del precio no puede exceder los 200 caracteres");

                    RuleFor(x => x.Modalidad)
                        .NotEmpty()
                        .WithMessage("La modalidad es requerida")
                        .MaximumLength(50)
                        .WithMessage("La modalidad no puede exceder los 50 caracteres");

                    RuleFor(x => x.CantidadMinima)
                        .GreaterThan(0)
                        .WithMessage("La cantidad mínima debe ser mayor a 0")
                        .When(x => x.CantidadMinima.HasValue);
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
                .MaximumLength(1000)
                .WithMessage("La descripción no puede exceder los 1000 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Descripcion));

            RuleFor(x => x.CategoriaID)
                .GreaterThan((short)0)
                .WithMessage("Debe seleccionar una categoría válida");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El stock debe ser mayor o igual a 0");

            RuleFor(x => x.StockMinimo)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El stock mínimo debe ser mayor o igual a 0")
                .When(x => x.StockMinimo.HasValue);

            RuleFor(x => x.NuevoPrecio)
                .GreaterThan(0)
                .WithMessage("El nuevo precio debe ser mayor a 0")
                .When(x => x.NuevoPrecio.HasValue);
        }
    }
}