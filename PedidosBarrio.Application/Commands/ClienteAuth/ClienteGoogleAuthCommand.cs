using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.ClienteAuth;

/// <summary>
/// Command para autenticación con Google
/// Valida el ID token y crea o autentica al cliente
/// </summary>
public class ClienteGoogleAuthCommand : IRequest<ClienteAuthResponseDto>
{
    public string IdToken { get; set; } = null!;
    public string DNI { get; set; } = null!;
    public string? Telefono { get; set; }
    public decimal? Latitud { get; set; }
    public decimal? Longitud { get; set; }

    public ClienteGoogleAuthCommand() { }

    public ClienteGoogleAuthCommand(string idToken, string dni, string? telefono = null, decimal? latitud = null, decimal? longitud = null)
    {
        IdToken = idToken;
        DNI = dni;
        Telefono = telefono;
        Latitud = latitud;
        Longitud = longitud;
    }
}
