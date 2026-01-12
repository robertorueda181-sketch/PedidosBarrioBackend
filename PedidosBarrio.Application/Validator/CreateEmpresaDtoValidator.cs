using FluentValidation;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Validator
{
    public class CreateEmpresaDtoValidator : AbstractValidator<CreateEmpresaDto>
    {
        private readonly IEmpresaRepository _empresaRepository;

        public CreateEmpresaDtoValidator(IEmpresaRepository empresaRepository)
        {
            _empresaRepository = empresaRepository;

            
        }
       
    }
}

