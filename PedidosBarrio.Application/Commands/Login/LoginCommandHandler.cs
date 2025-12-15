using FluentValidation;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Utilities;
using PedidosBarrio.Application.Validator;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IApplicationLogger _logger;
        private readonly LoginDtoValidator _validator;

        public LoginCommandHandler(
            IUsuarioRepository usuarioRepository,
            IApplicationLogger logger)
        {
            _usuarioRepository = usuarioRepository;
            _logger = logger;
            _validator = new LoginDtoValidator();
        }

        public async Task<LoginResponseDto> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            await _logger.LogInformationAsync($"Intento de login para email: {command.Email}", "LoginCommand");

            var loginDto = new LoginDto
            {
                Email = command.Email,
                Contrasena = command.Contrasena
            };

            var validationResult = await _validator.ValidateAsync(loginDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            await _logger.LogDebugAsync($"Validación exitosa para login: {command.Email}", "LoginCommand");

            // Buscar el usuario por email
            var usuario = await _usuarioRepository.GetByEmailAsync(command.Email);
            if (usuario == null)
            {
                await _logger.LogInformationAsync($"Usuario no encontrado para email: {command.Email}", "LoginCommand");
                throw new UnauthorizedAccessException("Email o contraseña inválidos.");
            }

            if (!usuario.Activa)
            {
                await _logger.LogInformationAsync($"Usuario inactivo intenta login: {command.Email}", "LoginCommand");
                throw new UnauthorizedAccessException("La cuenta está inactiva.");
            }

            // Verificar la contraseña
            var passwordValid = PasswordHasher.VerifyPassword(command.Contrasena, usuario.ContrasenaHash, usuario.ContrasenaSalt);
            if (!passwordValid)
            {
                await _logger.LogInformationAsync($"Contraseña incorrecta para usuario: {command.Email}", "LoginCommand");
                throw new UnauthorizedAccessException("Email o contraseña inválidos.");
            }

            await _logger.LogInformationAsync($"Login exitoso para usuario: {command.Email} (ID: {usuario.ID})", "LoginCommand");

            return new LoginResponseDto
            {
                UsuarioID = usuario.ID,
                NombreUsuario = usuario.NombreUsuario,
                Mensaje = "Login exitoso"
            };
        }
    }
}
