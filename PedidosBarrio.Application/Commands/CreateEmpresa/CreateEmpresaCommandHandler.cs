using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Utilities;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.CreateEmpresa
{
    public class CreateEmpresaCommandHandler : IRequestHandler<CreateEmpresaCommand, EmpresaDto>
    {
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateEmpresaDto> _createEmpresaDtoValidator;
        private readonly IApplicationLogger _logger;

        public CreateEmpresaCommandHandler(
            IEmpresaRepository empresaRepository,
            IUsuarioRepository usuarioRepository,
            IMapper mapper, 
            IValidator<CreateEmpresaDto> createEmpresaDtoValidator,
            IApplicationLogger logger)
        {
            _empresaRepository = empresaRepository;
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
            _createEmpresaDtoValidator = createEmpresaDtoValidator;
            _logger = logger;
        }

        public async Task<EmpresaDto> Handle(CreateEmpresaCommand command, CancellationToken cancellationToken)
        {
            await _logger.LogInformationAsync($"Creando empresa:", "CreateEmpresaCommand");

            var createDto = new CreateEmpresaDto
            {
                UsuarioID = command.UsuarioID,
                TipoEmpresa = command.TipoEmpresa
            };

            ValidationResult validationResult = await _createEmpresaDtoValidator.ValidateAsync(createDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var empresa = _mapper.Map<Empresa>(createDto);
            

            // Crear la empresa primero
            await _empresaRepository.AddAsync(empresa);

            // Crear el usuario asociado a la empresa
          

            return _mapper.Map<EmpresaDto>(empresa);
        }
    }
}
