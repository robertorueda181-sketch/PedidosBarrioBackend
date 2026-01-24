using MediatR;
using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.Commands.RegisterSocial;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Api.EndPoint
{
    public static class RegisterEndpoint
    {
        public static void MapRegisterEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/Auth/Register")
                           .WithTags("Autenticación - Registro");

            // POST /api/Auth/Register/business
            // Registro completo: usuario/contraseña O Google + empresa
            group.MapPost("/business", RegisterSocial)
                .WithName("RegisterSocial")
                .WithOpenApi()
                .Produces<LoginResponseDto>(StatusCodes.Status201Created)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                .WithDescription("Registrar nuevo usuario con empresa (usuario/contraseña o Google)")
                .WithSummary("Registro Completo");
        }

        private static async Task<IResult> RegisterSocial(
            [FromBody] RegisterSocialRequestDto request,
            IMediator mediator)
        {
            var command = new RegisterSocialCommand(
                email: request.Email,
                nombre: request.Nombre,
                apellido: request.Apellido,
                contrasena: request.Contrasena,
                nombreEmpresa: request.NombreEmpresa,
                tipoEmpresa: request.TipoEmpresa,
                categoria: request.Categoria,
                telefono: request.Telefono,
                descripcion: request.Descripcion,
                direccion: request.Direccion,
                referencia: request.Referencia,
                provider: request.Provider,
                socialId: request.SocialId,
                idToken: request.IdToken);

            var response = await mediator.Send(command);
            return Results.Created($"/api/Auth/usuario/{response.UsuarioID}", response);
        }
    }
}
