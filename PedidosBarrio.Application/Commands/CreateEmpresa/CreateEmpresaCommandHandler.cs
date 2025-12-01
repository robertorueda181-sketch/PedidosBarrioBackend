using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Utilities;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.CreateEmpresa
{
    public class CreateEmpresaCommandHandler : IRequestHandler<CreateEmpresaCommand, EmpresaDto>
    {
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateEmpresaDto> _createEmpresaDtoValidator;

        public CreateEmpresaCommandHandler(IEmpresaRepository empresaRepository, IMapper mapper, IValidator<CreateEmpresaDto> createEmpresaDtoValidator)
        {
            _empresaRepository = empresaRepository;
            _mapper = mapper;
            _createEmpresaDtoValidator = createEmpresaDtoValidator;
        }

        public async Task<EmpresaDto> Handle(CreateEmpresaCommand command, CancellationToken cancellationToken)
        {
            var createDto = new CreateEmpresaDto
            {
                Nombre = command.Nombre,
                Descripcion = command.Descripcion,
                Direccion = command.Direccion,
                Referencia = command.Referencia,
                Email = command.Email,
                Contrasena = command.ContrasenaHash, // Recibe la contraseña en texto plano desde el comando
                Telefono = command.Telefono
            };

            ValidationResult validationResult = await _createEmpresaDtoValidator.ValidateAsync(createDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new FluentValidation.ValidationException(validationResult.Errors);
            }

            // Generar hash y salt de la contraseña
            var (passwordHash, passwordSalt) = PasswordHasher.HashPassword(command.ContrasenaHash);

            var empresa = new Empresa(
                command.Nombre, 
                command.Email, 
                passwordHash, 
                passwordSalt, 
                command.Telefono
            );

            await _empresaRepository.AddAsync(empresa);
            return _mapper.Map<EmpresaDto>(empresa);
        }
    }
}
