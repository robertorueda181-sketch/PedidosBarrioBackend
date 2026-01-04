using FluentValidation;
using FluentValidation.Results;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Utilities;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.Register
{
    /// <summary>
    /// Handler para registrar una nueva empresa con usuario
    /// Maneja toda la lógica de registro completo
    /// </summary>
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, LoginResponseDto>
    {
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IApplicationLogger _logger;
        private readonly IValidator<RegisterDto> _validator;

        public RegisterCommandHandler(
            IEmpresaRepository empresaRepository,
            IUsuarioRepository usuarioRepository,
            IApplicationLogger logger,
            IValidator<RegisterDto> validator)
        {
            _empresaRepository = empresaRepository;
            _usuarioRepository = usuarioRepository;
            _logger = logger;
            _validator = validator;
        }

        public async Task<LoginResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _logger.LogInformationAsync(
                    $"Iniciando registro de empresa: {request.BusinessName}",
                    "RegisterCommand");

                // Validar datos de entrada
                var validateDto = new RegisterDto
                {
                    Fullname = request.Fullname,
                    DNI = request.DNI,
                    BusinessName = request.BusinessName,
                    RUC = request.RUC,
                    Category = request.Category,
                    Description = request.Description,
                    Schedules = request.Schedules,
                    Address = request.Address,
                    UseMap = request.UseMap,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    Reference = request.Reference,
                    Phone = request.Phone,
                    Email = request.Email,
                    Username = request.Username,
                    Password = request.Password
                };

                ValidationResult validationResult = await _validator.ValidateAsync(validateDto, cancellationToken);
                if (!validationResult.IsValid)
                {
                    await _logger.LogWarningAsync(
                        $"Validación fallida en registro: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}",
                        "RegisterCommand");
                    throw new ValidationException(validationResult.Errors);
                }

                await _logger.LogDebugAsync("Validación exitosa en registro", "RegisterCommand");

                // Verificar si el email ya existe
                var usuarioExistente = await _usuarioRepository.GetByEmailAsync(request.Email);
                if (usuarioExistente != null)
                {
                    await _logger.LogWarningAsync(
                        $"Intento de registro con email ya existente: {request.Email}",
                        "RegisterCommand");
                    throw new ApplicationException($"El email {request.Email} ya está registrado");
                }

                // Verificar si el username ya existe
                var usuarioByUsername = await _usuarioRepository.GetByNombreUsuarioAsync(request.Username);
                if (usuarioByUsername != null)
                {
                    await _logger.LogWarningAsync(
                        $"Intento de registro con username ya existente: {request.Username}",
                        "RegisterCommand");
                    throw new ApplicationException($"El usuario {request.Username} ya está registrado");
                }

                // Crear la empresa
                var (passwordHash, passwordSalt) = PasswordHasher.HashPassword(request.Password);

                var empresa = new Empresa(
                    nombre: request.BusinessName,
                    email: request.Email,
                    contrasenaHash: passwordHash,
                    contrasenaSalt: passwordSalt,
                    telefono: request.Phone)
                {
                    Descripcion = request.Description,
                    Direccion = request.Address,
                    Referencia = request.Reference,
                    Activa = true,
                    FechaRegistro = DateTime.UtcNow
                };

                // Si usa mapa, agregar coordenadas a la dirección
                if (request.UseMap && request.Latitude.HasValue && request.Longitude.HasValue)
                {
                    empresa.Direccion = $"{empresa.Direccion} (Lat: {request.Latitude}, Lng: {request.Longitude})";
                }

                await _empresaRepository.AddAsync(empresa);
                await _logger.LogInformationAsync(
                    $"Empresa creada: {empresa.Nombre} (ID: {empresa.ID})",
                    "RegisterCommand");

                // Crear el usuario asociado
                var usuario = new Usuario(
                    nombreUsuario: request.Username,
                    email: request.Email,
                    contrasenaHash: passwordHash,
                    contrasenaSalt: passwordSalt,
                    empresaID: empresa.ID)
                {
                    Activa = true,
                    FechaRegistro = DateTime.UtcNow
                };

                await _usuarioRepository.AddAsync(usuario);
                await _logger.LogInformationAsync(
                    $"Usuario creado para empresa: {usuario.NombreUsuario} (ID: {usuario.ID})",
                    "RegisterCommand");

                // Crear respuesta de login
                var loginResponse = new LoginResponseDto
                {
                    UsuarioID = usuario.ID,
                    EmpresaID = empresa.ID,
                    NombreUsuario = usuario.NombreUsuario,
                    Email = usuario.Email,
                    Mensaje = "Registro exitoso"
                };

                await _logger.LogInformationAsync(
                    $"Registro completado exitosamente para {request.BusinessName}",
                    "RegisterCommand");

                return loginResponse;
            }
            catch (ValidationException ex)
            {
                await _logger.LogErrorAsync(
                    $"Error de validación en registro: {string.Join(", ", ex.Errors.Select(e => e.ErrorMessage))}",
                    null,
                    "RegisterCommand");
                throw;
            }
            catch (ApplicationException ex)
            {
                await _logger.LogErrorAsync(
                    $"Error de aplicación en registro: {ex.Message}",
                    ex,
                    "RegisterCommand");
                throw;
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(
                    $"Error inesperado en registro: {ex.Message}",
                    ex,
                    "RegisterCommand");
                throw new ApplicationException("Error al registrar la empresa. Por favor intente más tarde.", ex);
            }
        }
    }
}
