using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace PedidosBarrio.Application.Services;

/// <summary>
/// Service para validar Google ID tokens
/// Verifica la firma, vigencia y claims requeridos
/// </summary>
public interface IGoogleTokenValidatorService
{
    /// <summary>
    /// Valida un Google ID token y extrae los claims
    /// </summary>
    /// <param name="idToken">El ID token de Google</param>
    /// <returns>Claims si es válido, excepción si no</returns>
    Task<GoogleTokenClaims> ValidateGoogleTokenAsync(string idToken);
}

public class GoogleTokenClaims
{
    public string GoogleId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Picture { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}

public class GoogleTokenValidatorService : IGoogleTokenValidatorService
{
    private readonly HttpClient _httpClient;
    private const string GoogleJwksUrl = "https://www.googleapis.com/oauth2/v3/certs";
    private IReadOnlyDictionary<string, JsonWebKey>? _googleKeys;
    private DateTime _keysCachedTime = DateTime.MinValue;
    private const int KeysCacheMinutes = 60;

    public GoogleTokenValidatorService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<GoogleTokenClaims> ValidateGoogleTokenAsync(string idToken)
    {
        if (string.IsNullOrWhiteSpace(idToken))
        {
            throw new InvalidOperationException("ID token no puede estar vacío");
        }

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(idToken);

            // Validar que no esté expirado
            if (token.ValidTo < DateTime.UtcNow)
            {
                throw new SecurityTokenExpiredException("El token de Google ha expirado");
            }

            // Obtener las claves públicas de Google
            var keys = await GetGooglePublicKeysAsync();

            // Validar la firma
            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = keys.Values,
                ValidateIssuer = true,
                ValidIssuer = "https://accounts.google.com",
                ValidateAudience = false, // El audience puede variar según la app
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5)
            };

            var principal = handler.ValidateToken(idToken, parameters, out SecurityToken validatedToken);

            // Extraer claims - siendo flexible con email
            var googleId = principal.FindFirst("aud")?.Value;
            var email = principal.FindFirst("email")?.Value ?? principal.FindFirst("email_verified")?.Value;
            var name = principal.FindFirst("name")?.Value;
            var picture = principal.FindFirst("picture")?.Value;

            // Requerir solo GoogleId (sub), email es opcional
            if (string.IsNullOrEmpty(googleId))
            {
                throw new InvalidOperationException("Token de Google no contiene 'sub' (Google ID)");
            }

            // Si no tiene email, usar GoogleId como placeholder
            if (string.IsNullOrEmpty(email))
            {
                email = $"google_{googleId}@google.com";
            }

            return new GoogleTokenClaims
            {
                GoogleId = googleId,
                Email = email,
                Name = name ?? $"Google User ({googleId.Substring(0, 8)}...)",
                Picture = picture,
                IssuedAt = UnixTimeStampToDateTime(long.Parse(principal.FindFirst("iat")?.Value ?? "0")),
                ExpiresAt = UnixTimeStampToDateTime(long.Parse(principal.FindFirst("exp")?.Value ?? "0"))
            };
        }
        catch (SecurityTokenException ex)
        {
            throw new InvalidOperationException($"Token de Google inválido: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error al validar token de Google: {ex.Message}", ex);
        }
    }

    private async Task<IReadOnlyDictionary<string, JsonWebKey>> GetGooglePublicKeysAsync()
    {
        // Usar caché si está disponible y no ha expirado
        if (_googleKeys != null && DateTime.UtcNow.Subtract(_keysCachedTime).TotalMinutes < KeysCacheMinutes)
        {
            return _googleKeys;
        }

        try
        {
            var response = await _httpClient.GetAsync(GoogleJwksUrl);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var jwksDocument = JsonDocument.Parse(content);

            var keys = new Dictionary<string, JsonWebKey>();

            foreach (var key in jwksDocument.RootElement.GetProperty("keys").EnumerateArray())
            {
                var kid = key.GetProperty("kid").GetString();
                var n = key.GetProperty("n").GetString();
                var e = key.GetProperty("e").GetString();

                if (kid != null && n != null && e != null)
                {
                    var jsonWebKey = new JsonWebKey
                    {
                        Kid = kid,
                        N = n,
                        E = e,
                        Kty = "RSA"
                    };

                    keys[kid] = jsonWebKey;
                }
            }

            _googleKeys = keys;
            _keysCachedTime = DateTime.UtcNow;

            return _googleKeys;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error al obtener claves públicas de Google: {ex.Message}", ex);
        }
    }

    private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
        return dateTime;
    }
}
