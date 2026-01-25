using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Application.Services
{
    /// <summary>
    /// Servicio para generar y validar JWT tokens
    /// </summary>
    public interface IJwtTokenService
    {
        string GenerateToken(Usuario usuario, int minutosExpiracion, string? emailOverride = null);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }

    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly string _jwtSecret;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            _jwtSecret = configuration.GetSection("Jwt:Secret").Value 
                ?? throw new InvalidOperationException("Jwt:Secret not configured");
            _jwtIssuer = configuration.GetSection("Jwt:Issuer").Value 
                ?? "PedidosBarrio";
            _jwtAudience = configuration.GetSection("Jwt:Audience").Value 
                ?? "PedidosBarrioApp";
        }

        /// <summary>
        /// Genera un JWT token para el usuario
        /// </summary>
        public string GenerateToken(Usuario usuario, int minutosExpiracion, string? emailOverride = null)
        {
            var email = emailOverride ?? usuario.Email;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.ID.ToString()),
                new Claim("UsuarioID", usuario.ID.ToString()), // Claim adicional para consistencia
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, email),
                new Claim("NombreCompleto", email),
                new Claim("EmpresaID", usuario.EmpresaID.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(minutosExpiracion),
                Issuer = _jwtIssuer,
                Audience = _jwtAudience,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Genera un refresh token (random string)
        /// </summary>
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        /// <summary>
        /// Obtiene el principal del token expirado para validar refresh token
        /// </summary>
        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret)),
                ValidateLifetime = false // Ignorar expiración para refresh token
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
    }
}
