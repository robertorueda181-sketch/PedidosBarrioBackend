using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using Microsoft.Extensions.Configuration;

namespace PedidosBarrio.Application.Commands.SocialLogin
{
    /// <summary>
    /// Handler para login social
    /// Registra usuario si no existe, loguea si existe
    /// Genera JWT token en ambos casos
    /// </summary>
    public class SocialLoginCommandHandler : IRequestHandler<SocialLoginCommand, SocialLoginResponseDto>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IApplicationLogger _logger;
        private readonly IConfiguration _configuration;

        public SocialLoginCommandHandler(
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
        }

        public async Task<SocialLoginResponseDto> Handle(SocialLoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _logger.LogInformationAsync(
                    $"Iniciando login social: {request.Provider} - {request.Email}",
                    "SocialLoginCommand");

                // Obtener minutos de expiración del token desde appsettings
                var tokenExpirationMinutes = _configuration.GetSection("Jwt:TokenExpirationMinutes").Value;
                int minutosExpiracion = !string.IsNullOrEmpty(tokenExpirationMinutes) && int.TryParse(tokenExpirationMinutes, out int minutos)
                    ? minutos
                    : 30; // Default 30 minutos

                // Buscar usuario por email
                var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);
                bool esNuevoUsuario = false;

                if (usuario == null)
                {
                    // Usuario no existe, registrarlo
                    await _logger.LogInformationAsync(
                        $"Nuevo usuario social: {request.Email} ({request.Provider})",
                        "SocialLoginCommand");

                    // Generar username basado en email
                    var username = request.Email.Split('@')[0];
                    var counter = 1;
                    var originalUsername = username;
                    
                    // Crear usuario sin contraseña (login social)
                    usuario = new Usuario(
                        email: request.Email,
                        contrasenaHash: "", // No hay contraseña para login social
                        contrasenaSalt: "") // Usuario sin empresa asociada inicialmente
                    {
                        Activa = true,
                        FechaRegistro = DateTime.UtcNow
                    };

                    await _usuarioRepository.AddAsync(usuario);
                    esNuevoUsuario = true;

                    await _logger.LogInformationAsync(
                        $"Usuario social registrado: {usuario.ID} - {request.Email}",
                        "SocialLoginCommand");
                }
                else
                {
                    // Usuario existe, solo loguear
                    await _logger.LogInformationAsync(
                        $"Login social exitoso: {usuario.ID} - {request.Email}",
                        "SocialLoginCommand");
                }

                // Generar JWT token
                var token = _jwtTokenService.GenerateToken(usuario, minutosExpiracion);
                var refreshToken = _jwtTokenService.GenerateRefreshToken();
                var tokenExpiracion = DateTime.UtcNow.AddMinutes(minutosExpiracion);

               
                // Construir respuesta
                var response = new SocialLoginResponseDto
                {
                    UsuarioID = usuario.ID,
                    Email = usuario.Email,
                    NombreCompleto = $"{request.FirstName} {request.LastName}",
                    EmpresaID = usuario.EmpresaID,
                    Provider = request.Provider,
                    EsNuevoUsuario = esNuevoUsuario,
                    Token = token,
                    TipoEmpresa = "tipoEmpresaStr",
                    TokenExpiracion = tokenExpiracion,
                    RefreshToken = refreshToken
                };

                await _logger.LogInformationAsync(
                    $"Token generado para: {usuario.Email} ({request.Provider})",
                    "SocialLoginCommand");

                return response;
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(
                    $"Error en login social: {ex.Message}",
                    ex,
                    "SocialLoginCommand");
                throw new ApplicationException($"Error al realizar login social: {ex.Message}", ex);
            }
        }
    }
}
