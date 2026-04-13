using FluentValidation;
using PedidosBarrio.Application.Queries.GetPaginaByCodigo;

namespace PedidosBarrio.Application.Validators
{
    public class GetPaginaByCodigoValidator : AbstractValidator<GetPaginaByCodigoQuery>
    {
        public GetPaginaByCodigoValidator()
        {
            RuleFor(x => x.Codigo)
                .NotEmpty().WithMessage("Código del negocio es requerido")
                .MaximumLength(100).WithMessage("Código no puede exceder 100 caracteres");
        }
    }
}
