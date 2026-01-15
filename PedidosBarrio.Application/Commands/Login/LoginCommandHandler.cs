using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Application.Utilities;
using PedidosBarrio.Application.Validator;
using PedidosBarrio.Domain.Repositories;
using Microsoft.Extensions.Configuration;

namespace PedidosBarrio.Application.Commands.Login
{
    /// <summary>
    /// Handler para login unificado
    /// Soporta: usuario/contraseña O Google
    /// 1. Valida credenciales
    /// 2. Obtiene datos del usuario y empresa
    /// 3. Genera JWT token
    /// </summary>
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IApplicationLogger _logger;
        private readonly IConfiguration _configuration;
        private readonly LoginDtoValidator _validator;

        public LoginCommandHandler(
            IUsuarioRepository usuarioRepository,
            IEmpresaRepository empresaRepository,
            IJwtTokenService jwtTokenService,
            IApplicationLogger logger,
            IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository;
            _empresaRepository = empresaRepository;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
            _configuration = configuration;
            _validator = new LoginDtoValidator();
        }

        public async Task<LoginResponseDto> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            try
            {
                await _logger.LogInformationAsync(
                    $"Intento de login: {command.Email} - Provider: {command.Provider ?? "usuario/contraseña"}",
                    "LoginCommand");

                // ===== 1. BUSCAR USUARIO POR EMAIL =====
                var usuario = await _usuarioRepository.GetByEmailAsync(command.Email);

                var empresa = await _empresaRepository.GetByIdAsync(usuario.EmpresaID);

                if (usuario == null)
                {
                    if (command.Provider == "google")
                    {
                        throw new ApplicationException("No se ha registrado");
                    }
                    await _logger.LogWarningAsync(
                    $"Intento de login fallido - Usuario no encontrado: {command.Email}",
                    "LoginCommand");
                    throw new ApplicationException("Email o contraseña inválidos.");
                }

                // ===== 2. VALIDAR CREDENCIALES SEGÚN TIPO DE LOGIN =====
                if (string.IsNullOrEmpty(command.Provider))
                {
                    // Login por usuario/contraseña
                    await ValidarCredencialesUsuarioContrasena(usuario, command.Contrasena);
                    await _logger.LogInformationAsync(
                        $"Login exitoso por usuario/contraseña: {usuario.Email}",
                        "LoginCommand");
                }
                else if (command.Provider == "google")
                {
                    // Login por Google
                    if (!command.SocialId.Equals(usuario.SocialId))
                    {
                        throw new ApplicationException("IdToken de Google es incorrecto");
                    }

                    // TODO: Validar IdToken con Google en producción
                    // Por ahora, solo verificamos que el usuario exista
                    await _logger.LogInformationAsync(
                        $"Login exitoso por Google: {command.Email}",
                        "LoginCommand");
                }
                else
                {
                    throw new ApplicationException("Provider no soportado.");
                }


                // ===== 4. GENERAR TOKENS =====
                var tokenExpirationMinutes = _configuration.GetSection("Jwt:TokenExpirationMinutes").Value;
                int minutosExpiracion = !string.IsNullOrEmpty(tokenExpirationMinutes) && int.TryParse(tokenExpirationMinutes, out int minutos)
                    ? minutos
                    : 30;
                usuario.Email = command.Email;
                var token = _jwtTokenService.GenerateToken(usuario, minutosExpiracion);
                var refreshToken = _jwtTokenService.GenerateRefreshToken();



                var tipoEmpresaStr = empresa.TipoEmpresa switch
                {
                    1 => "NEGOCIO",
                    2 => "SERVICIO",
                    3 => "INMUEBLE",
                    _ => "DESCONOCIDO"
                };


                // ===== 5. CONSTRUIR RESPUESTA =====
                var response = new LoginResponseDto
                {
                    UsuarioID = usuario.ID,
                    Email = usuario.Email,
                    NombreUsuario = usuario.NombreUsuario,
                    NombreCompleto = usuario.NombreUsuario,
                    EmpresaID = usuario.EmpresaID,
                    NombreEmpresa = "Sin empresa",
                    TipoEmpresa = tipoEmpresaStr,
                    Telefono = null,
                    Token = token,
                    RefreshToken = refreshToken,
                    TokenExpiracion = DateTime.UtcNow.AddMinutes(minutosExpiracion),
                    EsNuevo = false,
                    Mensaje = "Login exitoso"
                };

                await _logger.LogInformationAsync(
                    $"Token generado para: {usuario.Email}",
                    "LoginCommand");

                return response;
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(
                    $"Error en login: {ex.Message}",
                    ex,
                    "LoginCommand");
                throw new ApplicationException($"Error al iniciar sesión: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Valida credenciales de usuario/contraseña
        /// </summary>
        private async Task ValidarCredencialesUsuarioContrasena(Domain.Entities.Usuario usuario, string contrasena)
        {
            if (string.IsNullOrEmpty(contrasena))
            {
                throw new ApplicationException("Contraseña requerida.");
            }

            // Verificar que el usuario tiene contraseña (no es solo social login)
            if (string.IsNullOrEmpty(usuario.ContrasenaHash))
            {
                await _logger.LogWarningAsync(
                    $"Intento de login con contraseña en usuario solo Google: {usuario.Email}",
                    "LoginCommand");
                throw new ApplicationException("Este usuario solo tiene login por Google. Use Google para iniciar sesión.");
            }

            // Verificar contraseña
            var esValida = PasswordHasher.VerifyPassword(contrasena, usuario.ContrasenaHash, usuario.ContrasenaSalt);
            if (!esValida)
            {
                await _logger.LogWarningAsync(
                    $"Contraseña inválida para usuario: {usuario.Email}",
                    "LoginCommand");
                throw new ApplicationException("Email o contraseña inválidos.");
            }
        }

        /// <summary>
        /// Mapea el tipo de empresa numérico a string
        /// </summary>
        private string MapearTipoEmpresa(short tipoEmpresa)
        {
            return tipoEmpresa switch
            {
                1 => "NEGOCIO",
                2 => "SERVICIO",
                3 => "INMUEBLE",
                _ => "DESCONOCIDO"
            };
        }
    }
}
