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
            await _logger.LogInformationAsync($"Creando empresa: {command.Nombre}", "CreateEmpresaCommand");

            var createDto = new CreateEmpresaDto
            {
                Nombre = command.Nombre,
                Descripcion = command.Descripcion,
                Direccion = command.Direccion,
                Referencia = command.Referencia,
                Email = command.Email,
                Contrasena = command.Contrasena,
                Telefono = command.Telefono
            };

            ValidationResult validationResult = await _createEmpresaDtoValidator.ValidateAsync(createDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            await _logger.LogDebugAsync($"Validación exitosa para {command.Nombre}", "CreateEmpresaCommand");

            var empresa = _mapper.Map<Empresa>(createDto);
            
            // Generar hash y salt de la contraseña
            var (passwordHash, passwordSalt) = PasswordHasher.HashPassword(command.Contrasena);
            empresa.ContrasenaHash = passwordHash;
            empresa.ContrasenaSalt = passwordSalt;

            await _logger.LogDebugAsync($"Contraseña hasheada para {command.Email}", "CreateEmpresaCommand");

            // Crear la empresa primero
            await _empresaRepository.AddAsync(empresa);

            // Crear el usuario asociado a la empresa
            var usuario = new Usuario(
                nombreUsuario: command.Nombre.ToLower().Replace(" ", ""),
                email: command.Email,
                contrasenaHash: empresa.ContrasenaHash,
                contrasenaSalt: empresa.ContrasenaSalt,
                empresaID: empresa.ID);

            usuario.Activa = true;
            usuario.FechaRegistro = DateTime.UtcNow;

            await _usuarioRepository.AddAsync(usuario);

            await _logger.LogInformationAsync($"Empresa creada: {empresa.Nombre} (ID: {empresa.ID})", "CreateEmpresaCommand");
            await _logger.LogInformationAsync($"Usuario creado para empresa: {usuario.NombreUsuario} (ID: {usuario.ID})", "CreateEmpresaCommand");

            return _mapper.Map<EmpresaDto>(empresa);
        }
    }
}
