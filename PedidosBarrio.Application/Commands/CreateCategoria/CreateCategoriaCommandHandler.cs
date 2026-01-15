using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.CreateCategoria
{
    public class CreateCategoriaCommandHandler : IRequestHandler<CreateCategoriaCommand, CategoriaDto>
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateCategoriaDto> _validator;
        private readonly ICurrentUserService _currentUserService;

        public CreateCategoriaCommandHandler(
            ICategoriaRepository categoriaRepository, 
            IMapper mapper, 
            IValidator<CreateCategoriaDto> validator,
            ICurrentUserService currentUserService)
        {
            _categoriaRepository = categoriaRepository;
            _mapper = mapper;
            _validator = validator;
            _currentUserService = currentUserService;
        }

        public async Task<CategoriaDto> Handle(CreateCategoriaCommand command, CancellationToken cancellationToken)
        {
            // Obtener EmpresaID del token
            var empresaId = _currentUserService.GetEmpresaId();

            // Validate input
            var createDto = new CreateCategoriaDto
            {
                Descripcion = command.Descripcion,
                Color = command.Color
            };

            ValidationResult validationResult = await _validator.ValidateAsync(createDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var categoria = new Categoria(empresaId, command.Descripcion, command.Color);

            var categoriaId = await _categoriaRepository.AddAsync(categoria);

            return new CategoriaDto
            {
                CategoriaID = categoriaId,
                Descripcion = categoria.Descripcion,
                Color = categoria.Color,
                Activo = categoria.Activo
            };
        }
    }
}