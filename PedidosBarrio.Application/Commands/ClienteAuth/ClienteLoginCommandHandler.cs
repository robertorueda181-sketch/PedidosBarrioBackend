using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace PedidosBarrio.Application.Commands.ClienteAuth;

public class ClienteLoginCommandHandler : IRequestHandler<ClienteLoginCommand, ClienteAuthResponseDto>
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IApplicationLogger _logger;
    private readonly IJwtTokenService _jwtTokenService;

    public ClienteLoginCommandHandler(
        IClienteRepository clienteRepository,
        IApplicationLogger logger,
        IJwtTokenService jwtTokenService)
    {
        _clienteRepository = clienteRepository;
        _logger = logger;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<ClienteAuthResponseDto> Handle(ClienteLoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dni = request.Data.DNI?.Trim();
            var email = request.Data.Email?.Trim();
            var contrasena = request.Data.Contrasena;

            // Validaciones: DNI o Email + contraseña
            if ((string.IsNullOrEmpty(dni) && string.IsNullOrEmpty(email)) || 
                string.IsNullOrEmpty(contrasena))
            {
                return new ClienteAuthResponseDto
                {
                    Success = false,
                    Message = "DNI o Email y contraseña son requeridos"
                };
            }

            // Buscar cliente por DNI
            Cliente cliente = null;
            string identificador = "";

            if (!string.IsNullOrEmpty(dni))
            {
                cliente = await _clienteRepository.GetByDniAsync(dni);
                identificador = dni;
            }
            else if (!string.IsNullOrEmpty(email))
            {
                cliente = await _clienteRepository.GetByEmailAsync(email);
                identificador = email;
            }

            if (cliente == null)
            {
                return new ClienteAuthResponseDto
                {
                    Success = false,
                    Message = "Cliente no encontrado"
                };
            }

            // Verificar que el cliente esté activo
            if (!cliente.Activo)
            {
                return new ClienteAuthResponseDto
                {
                    Success = false,
                    Message = "Cliente inactivo"
                };
            }

            // Verificar contraseña
            if (!VerifyPassword(contrasena, cliente.ContrasenaHash, cliente.ContrasenaSalt))
            {
                await _logger.LogWarningAsync($"Intento de login fallido para cliente: {identificador}");
                return new ClienteAuthResponseDto
                {
                    Success = false,
                    Message = "Contraseña incorrecta"
                };
            }

            // Generar token JWT
            var token = _jwtTokenService.GenerateClienteToken(cliente.ClienteID, cliente.DNI);

            await _logger.LogInformationAsync($"Cliente autenticado exitosamente: {identificador}");

            return new ClienteAuthResponseDto
            {
                Success = true,
                Message = "Login exitoso",
                Data = new ClienteTokenDto
                {
                    ClienteID = cliente.ClienteID,
                    DNI = cliente.DNI,
                    Email = cliente.Email ?? string.Empty,
                    Nombres = cliente.Nombres,
                    Token = token
                }
            };
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync("Error al autenticar cliente", ex);
            return new ClienteAuthResponseDto
            {
                Success = false,
                Message = "Error al autenticar cliente"
            };
        }
    }

    private bool VerifyPassword(string password, string hash, string salt)
    {
        if (string.IsNullOrEmpty(hash) || string.IsNullOrEmpty(salt))
            return false;

        using (var pbkdf2 = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt), 10000, HashAlgorithmName.SHA256))
        {
            byte[] hashOfInput = pbkdf2.GetBytes(256);
            string hashString = Convert.ToBase64String(hashOfInput);
            return hashString == hash;
        }
    }
}
