using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace PedidosBarrio.Api.EndPoint;

/// <summary>
/// Endpoint de debug para diagnosticar tokens de Google
/// SOLO USAR EN DESARROLLO
/// </summary>
public static class TokenDebugEndpoint
{
    public static void MapTokenDebugEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/Debug/Token")
                       .WithTags("Debug - Token Analysis")
                       .WithName("TokenDebug");

        // GET /api/Debug/Token/Decode - Decodificar token sin validar firma
        group.MapPost("/Decode", DecodeToken)
            .WithName("DecodeToken")
            .WithOpenApi()
            .Produces<object>(StatusCodes.Status200OK)
            .WithSummary("Decodificar token JWT (sin validar firma)")
            .WithDescription("Útil para debug: decodifica cualquier JWT y muestra sus claims");
    }

    private static IResult DecodeToken([FromBody] TokenDebugRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Token))
            {
                return Results.BadRequest(new
                {
                    success = false,
                    message = "Token no puede estar vacío"
                });
            }

            var token = request.Token.Trim();

            // Verificar que sea un JWT válido (3 partes separadas por .)
            var parts = token.Split('.');
            if (parts.Length != 3)
            {
                return Results.BadRequest(new
                {
                    success = false,
                    message = "Token no es un JWT válido (debe tener 3 partes separadas por .)",
                    tokenLength = token.Length,
                    parts = parts.Length
                });
            }

            try
            {
                // Decodificar sin validar firma
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var payload = new Dictionary<string, object>();

                // Extraer todos los claims
                foreach (var claim in jwtToken.Claims)
                {
                    payload[claim.Type] = claim.Value;
                }

                // Información adicional
                var expiresAt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    .AddSeconds(long.Parse(payload.ContainsKey("exp") ? payload["exp"].ToString() : "0"));

                var issuedAt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    .AddSeconds(long.Parse(payload.ContainsKey("iat") ? payload["iat"].ToString() : "0"));

                var isExpired = expiresAt < DateTime.UtcNow;

                return Results.Ok(new
                {
                    success = true,
                    message = "Token decodificado exitosamente",
                    token = new
                    {
                        header = new
                        {
                            alg = jwtToken.Header.Alg,
                            typ = jwtToken.Header.Typ,
                            kid = jwtToken.Header.Kid
                        },
                        payload = payload,
                        claims = new
                        {
                            sub = payload.ContainsKey("sub") ? payload["sub"] : "❌ FALTA",
                            email = payload.ContainsKey("email") ? payload["email"] : "❌ FALTA",
                            name = payload.ContainsKey("name") ? payload["name"] : "❌ FALTA",
                            iss = payload.ContainsKey("iss") ? payload["iss"] : "❌ FALTA",
                            aud = payload.ContainsKey("aud") ? payload["aud"] : "❌ FALTA",
                            email_verified = payload.ContainsKey("email_verified") ? payload["email_verified"] : "❌ FALTA"
                        },
                        timing = new
                        {
                            issued_at = issuedAt,
                            expires_at = expiresAt,
                            is_expired = isExpired,
                            issued_at_readable = issuedAt.ToString("O"),
                            expires_at_readable = expiresAt.ToString("O"),
                            now_utc = DateTime.UtcNow.ToString("O")
                        },
                        critical_checks = new
                        {
                            has_sub = payload.ContainsKey("sub") ? "✅ SÍ" : "❌ NO",
                            has_email = payload.ContainsKey("email") ? "✅ SÍ" : "⚠️ NO (pero es opcional ahora)",
                            is_google_token = payload.ContainsKey("iss") && payload["iss"].ToString().Contains("google") ? "✅ SÍ" : "❌ NO",
                            is_expired = isExpired ? "❌ SÍ (EXPIRADO)" : "✅ NO",
                            is_valid_for_backend = !isExpired && payload.ContainsKey("sub") ? "✅ VÁLIDO" : "❌ INVÁLIDO"
                        }
                    }
                });
            }
            catch (Exception decodeEx)
            {
                return Results.BadRequest(new
                {
                    success = false,
                    message = "No se puede decodificar el token",
                    error = decodeEx.Message,
                    hint = "Verifica que sea un JWT válido (formato: header.payload.signature)"
                });
            }
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new
            {
                success = false,
                message = "Error al procesar token",
                error = ex.Message
            });
        }
    }
}

public class TokenDebugRequest
{
    public string Token { get; set; } = null!;
}
