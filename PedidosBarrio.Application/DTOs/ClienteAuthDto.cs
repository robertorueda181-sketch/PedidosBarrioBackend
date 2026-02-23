namespace PedidosBarrio.Application.DTOs;

public class ClienteLoginDto
{
    public string? DNI { get; set; }
    public string? Email { get; set; }
    public string? Contrasena { get; set; }

    // OAuth fields
    public string? Provider { get; set; } // "google", "facebook", etc.
    public string? IdToken { get; set; } // ID token from OAuth provider
    public string? GoogleId { get; set; } // Google user ID
}

public class ClienteRegistroDto
{
    public string? DNI { get; set; }
    public string? Nombres { get; set; }
    public string? Contrasena { get; set; }
    public string? Telefono { get; set; }

    // OAuth fields
    public string? Provider { get; set; } // "google", "facebook", etc.
    public string? IdToken { get; set; } // ID token from OAuth provider
    public string? GoogleId { get; set; } // Google user ID
    public string? Email { get; set; } // For OAuth providers

    // Ubicación geográfica (opcional)
    public decimal? Latitud { get; set; }
    public decimal? Longitud { get; set; }
}

public class ClienteAuthResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public ClienteTokenDto? Data { get; set; }
    /// <summary>
    /// Indica si es un cliente nuevo (true) o existente (false)
    /// Útil para retornar HTTP 201 (Created) o 200 (OK)
    /// </summary>
    public bool IsNewClient { get; set; } = false;
}

public class ClienteTokenDto
{
    public Guid ClienteID { get; set; }
    public string DNI { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Nombres { get; set; } = null!;
    public string Token { get; set; } = null!;
}
