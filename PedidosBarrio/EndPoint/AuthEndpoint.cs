using MediatR;
using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.Commands.SocialLogin;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Api.EndPoint
{
    public static class AuthEndpoint
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/Auth")
                           .WithTags("Autenticación");

            // POST /api/Auth/SocialLogin
            group.MapPost("/SocialLogin", async ([FromBody] SocialLoginRequest request, IMediator mediator) =>
            {
                var response = await mediator.Send(new SocialLoginCommand(request));
                return Results.Ok(response);
            })
            .WithName("SocialLogin")
            .WithOpenApi()
            .WithDescription("Login social (Google, Facebook, etc.). Registra si no existe, loguea si existe. Devuelve JWT token con 30 minutos de expiración")
            .Produces<SocialLoginResponseDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

            // POST /api/Auth/RefreshToken
            group.MapPost("/RefreshToken", async ([FromBody] RefreshTokenRequest request, IMediator mediator) =>
            {
                // Este endpoint se implementará después
                return Results.Ok(new { message = "Endpoint de refresh token en construcción" });
            })
            .WithName("RefreshToken")
            .WithOpenApi()
            .WithDescription("Refresca el JWT token usando el refresh token")
            .Produces<RefreshTokenResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);
        }
    }
}
