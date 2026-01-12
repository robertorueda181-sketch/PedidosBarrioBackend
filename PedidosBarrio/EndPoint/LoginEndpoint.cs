using MediatR;
using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.Commands.Login;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Api.EndPoint
{
    public static class LoginEndpoint
    {
        public static void MapLoginEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/Auth/Login")
                           .WithTags("Autenticación - Login");

            // POST /api/Auth/Login
            // Login unificado: usuario/contraseña O Google
            group.MapPost("/", LoginUnificado)
                .WithName("LoginUnificado")
                .WithOpenApi()
                .Produces<LoginResponseDto>(StatusCodes.Status200OK)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                .WithDescription("Login unificado - usuario/contraseña O Google")
                .WithSummary("Login Unificado");
        }

        /// <summary>
        /// Login unificado: soporta usuario/contraseña O Google
        /// 
        /// OPCIÓN 1: Login por usuario/contraseña
        /// {
        ///   "email": "juan@example.com",
        ///   "contrasena": "Pass123"
        /// }
        /// 
        /// OPCIÓN 2: Login por Google
        /// {
        ///   "email": "juan@gmail.com",
        ///   "provider": "google",
        ///   "idToken": "eyJhbGciOiJSUzI1NiI...",
        ///   "googleId": "123456789"
        /// }
        /// </summary>
        private static async Task<IResult> LoginUnificado(
            [FromBody] LoginUnifiedRequest request,
            IMediator mediator)
        {
            LoginCommand command;

            if (string.IsNullOrEmpty(request.Provider))
            {
                // Login por usuario/contraseña
                command = new LoginCommand(request.Email, request.Contrasena);
            }
            else if (request.Provider == "google")
            {
                // Login por Google
                command = new LoginCommand(request.Email, request.Provider, request.IdToken, request.GoogleId);
            }
            else
            {
                return Results.BadRequest(new { error = "Provider no soportado. Use 'google' o null para usuario/contraseña." });
            }

            var response = await mediator.Send(command);
            return Results.Ok(response);
        }
    }

    /// <summary>
    /// Request unificado para login
    /// </summary>
    public class LoginUnifiedRequest
    {
        // ===== USUARIO/CONTRASEÑA O GOOGLE =====
        public string Email { get; set; }
        public string Contrasena { get; set; }

        // ===== GOOGLE (OPCIONAL) =====
        public string Provider { get; set; } // "google" o null
        public string IdToken { get; set; }
        public string GoogleId { get; set; }
    }
}
