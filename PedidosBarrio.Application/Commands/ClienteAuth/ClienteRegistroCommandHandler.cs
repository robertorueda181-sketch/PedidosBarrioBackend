using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace PedidosBarrio.Application.Commands.ClienteAuth;

public class ClienteRegistroCommandHandler : IRequestHandler<ClienteRegistroCommand, ClienteAuthResponseDto>
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IApplicationLogger _logger;
    private readonly IJwtTokenService _jwtTokenService;
    private const string DefaultEmpresaId = "00000000-0000-0000-0000-000000000000"; // Usuario genérico para clientes

    public ClienteRegistroCommandHandler(
        IClienteRepository clienteRepository,
        IApplicationLogger logger,
        IJwtTokenService jwtTokenService)
    {
        _clienteRepository = clienteRepository;
        _logger = logger;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<ClienteAuthResponseDto> Handle(ClienteRegistroCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dni = request.Data.DNI?.Trim();
            var email = request.Data.Email?.Trim();
            var nombres = request.Data.Nombres?.Trim();
            var contrasena = request.Data.Contrasena;

            // Validaciones: DNI o Email + nombres + contraseña
            if ((string.IsNullOrEmpty(dni) && string.IsNullOrEmpty(email)) || 
                string.IsNullOrEmpty(nombres) || 
                string.IsNullOrEmpty(contrasena))
            {
                return new ClienteAuthResponseDto
                {
                    Success = false,
                    Message = "DNI o Email, nombres y contraseña son requeridos"
                };
            }

            // Verificar si el cliente ya existe por DNI
            if (!string.IsNullOrEmpty(dni))
            {
                var clienteExistentePorDni = await _clienteRepository.GetByDniAsync(dni);
                if (clienteExistentePorDni != null)
                {
                    return new ClienteAuthResponseDto
                    {
                        Success = false,
                        Message = "El cliente con este DNI ya existe"
                    };
                }
            }

            // Verificar si el cliente ya existe por Email
            if (!string.IsNullOrEmpty(email))
            {
                var clienteExistentePorEmail = await _clienteRepository.GetByEmailAsync(email);
                if (clienteExistentePorEmail != null)
                {
                    return new ClienteAuthResponseDto
                    {
                        Success = false,
                        Message = "El cliente con este Email ya existe"
                    };
                }
            }

            // Generar hash y salt de contraseña
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] tokenBuffer = new byte[128];
                rng.GetBytes(tokenBuffer);
                string salt = Convert.ToBase64String(tokenBuffer);

                using (var pbkdf2 = new Rfc2898DeriveBytes(contrasena, Encoding.UTF8.GetBytes(salt), 10000, HashAlgorithmName.SHA256))
                {
                    byte[] hash = pbkdf2.GetBytes(256);
                    string hashString = Convert.ToBase64String(hash);

                    // Si no hay DNI, usar email como identificador
                    var dniOEmail = dni ?? email;

                    // Crear cliente con contraseña hasheada
                    var nuevoCliente = new Cliente(Guid.Parse(DefaultEmpresaId), dniOEmail, nombres)
                    {
                        ContrasenaHash = hashString,
                        ContrasenaSalt = salt,
                        Email = email,
                        Telefono = request.Data.Telefono
                    };

                    await _clienteRepository.AddAsync(nuevoCliente);

                    // Generar token JWT
                    var token = _jwtTokenService.GenerateClienteToken(nuevoCliente.ClienteID, dniOEmail);

                    await _logger.LogInformationAsync($"Cliente registrado exitosamente: {dniOEmail}");

                    return new ClienteAuthResponseDto
                    {
                        Success = true,
                        Message = "Cliente registrado exitosamente",
                        Data = new ClienteTokenDto
                        {
                            ClienteID = nuevoCliente.ClienteID,
                            DNI = nuevoCliente.DNI,
                            Email = nuevoCliente.Email ?? string.Empty,
                            Nombres = nuevoCliente.Nombres,
                            Token = token
                        }
                    };
                }
            }
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync($"Error al registrar cliente: {ex.Message}", ex);
            return new ClienteAuthResponseDto
            {
                Success = false,
                Message = "Error al registrar cliente",
            };
        }
    }
}
