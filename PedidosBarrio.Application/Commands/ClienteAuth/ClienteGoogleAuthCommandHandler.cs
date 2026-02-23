using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.ClienteAuth;

/// <summary>
/// Handler para autenticación con Google
/// 1. Valida el ID token de Google
/// 2. Busca cliente existente por GoogleId (ProviderUserId)
/// 3. Si existe, lo autentica
/// 4. Si no existe, crea uno nuevo
/// 5. Retorna JWT token para el cliente
/// </summary>
public class ClienteGoogleAuthCommandHandler : IRequestHandler<ClienteGoogleAuthCommand, ClienteAuthResponseDto>
{
    private readonly IGoogleTokenValidatorService _googleTokenValidator;
    private readonly IClienteRepository _clienteRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IApplicationLogger _logger;

    public ClienteGoogleAuthCommandHandler(
        IGoogleTokenValidatorService googleTokenValidator,
        IClienteRepository clienteRepository,
        IUsuarioRepository usuarioRepository,
        IJwtTokenService jwtTokenService,
        IApplicationLogger logger)
    {
        _googleTokenValidator = googleTokenValidator;
        _clienteRepository = clienteRepository;
        _usuarioRepository = usuarioRepository;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    public async Task<ClienteAuthResponseDto> Handle(ClienteGoogleAuthCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validar el token de Google
            GoogleTokenClaims googleClaims;
            try
            {
                googleClaims = await _googleTokenValidator.ValidateGoogleTokenAsync(request.IdToken);
            }
            catch (Exception ex)
            {
                await _logger.LogWarningAsync(
                    $"Error validando Google token: {ex.Message}",
                    "ClienteGoogleAuthCommand");
                return new ClienteAuthResponseDto
                {
                    Success = false,
                    Message = $"Token de Google inválido: {ex.Message}",
                    Data = null
                };
            }

            // 1. Buscar o crear Usuario
            var usuario = await _usuarioRepository.GetByEmailAsync(googleClaims.Email);

            if (usuario == null)
            {
                // Crear nuevo usuario con email de Google
                usuario = new Usuario(
                    email: googleClaims.Email,
                    contrasenaHash: "OAUTH_GOOGLE", // Placeholder para OAuth
                    contrasenaSalt: "OAUTH_GOOGLE"  // Placeholder para OAuth
                );

                // Establecer datos de OAuth
                usuario.Provider = "google";
                usuario.SocialId = googleClaims.GoogleId;

                await _usuarioRepository.AddAsync(usuario);

                await _logger.LogInformationAsync(
                    $"Nuevo usuario creado por Google: {usuario.Email}",
                    "ClienteGoogleAuthCommand");
            }

            // 2. Buscar cliente existente por UsuarioId o por ProviderUserId
            var cliente = await _clienteRepository.GetByProviderUserIdAsync("google", googleClaims.GoogleId);

            if (cliente == null)
            {
                // Crear nuevo cliente asociado al usuario
                cliente = new Cliente(
                    usuarioId: usuario.ID, // Asociar al usuario
                    dni: request.DNI,
                    nombres: googleClaims.Name
                );

                // Establecer datos de OAuth
                cliente.Provider = "google";
                cliente.ProviderUserId = googleClaims.GoogleId;
                cliente.Email = googleClaims.Email;
                cliente.Telefono = request.Telefono;

                // Establecer ubicación si se proporciona
                if (request.Latitud.HasValue && request.Longitud.HasValue)
                {
                    cliente.Latitud = request.Latitud;
                    cliente.Longitud = request.Longitud;
                }

                await _clienteRepository.AddAsync(cliente);

                await _logger.LogInformationAsync(
                    $"Nuevo cliente creado por Google: {cliente.Email} (UsuarioID: {usuario.ID})",
                    "ClienteGoogleAuthCommand");

                // Generar JWT token
                var newToken = _jwtTokenService.GenerateClienteToken(
                    cliente.ClienteID,
                    cliente.DNI,
                    1440 // 24 hours
                );

                return new ClienteAuthResponseDto
                {
                    Success = true,
                    Message = "Cliente nuevo registrado con Google exitosamente",
                    IsNewClient = true,
                    Data = new ClienteTokenDto
                    {
                        ClienteID = cliente.ClienteID,
                        DNI = cliente.DNI,
                        Email = cliente.Email ?? string.Empty,
                        Nombres = cliente.Nombres,
                        Token = newToken
                    }
                };
            }
            else
            {
                // Actualizar información del cliente existente
                bool updated = false;

                if (!string.IsNullOrWhiteSpace(request.Telefono) && cliente.Telefono != request.Telefono)
                {
                    cliente.Telefono = request.Telefono;
                    updated = true;
                }

                if (request.Latitud.HasValue && request.Longitud.HasValue)
                {
                    if (cliente.Latitud != request.Latitud || cliente.Longitud != request.Longitud)
                    {
                        cliente.Latitud = request.Latitud;
                        cliente.Longitud = request.Longitud;
                        updated = true;
                    }
                }

                if (updated)
                {
                    await _clienteRepository.UpdateAsync(cliente);
                }

                await _logger.LogInformationAsync(
                    $"Cliente autenticado por Google: {cliente.Email} (UsuarioID: {cliente.UsuarioID})",
                    "ClienteGoogleAuthCommand");

                // Generar JWT token
                var existingToken = _jwtTokenService.GenerateClienteToken(
                    cliente.ClienteID,
                    cliente.DNI,
                    1440 // 24 hours
                );

                return new ClienteAuthResponseDto
                {
                    Success = true,
                    Message = "Autenticación con Google exitosa",
                    IsNewClient = false,
                    Data = new ClienteTokenDto
                    {
                        ClienteID = cliente.ClienteID,
                        DNI = cliente.DNI,
                        Email = cliente.Email ?? string.Empty,
                        Nombres = cliente.Nombres,
                        Token = existingToken
                    }
                };
            }
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync(
                "Error en ClienteGoogleAuthCommandHandler",
                ex,
                "ClienteGoogleAuthCommand");

            return new ClienteAuthResponseDto
            {
                Success = false,
                Message = "Error al autenticar con Google",
                Data = null
            };
        }
    }
}
