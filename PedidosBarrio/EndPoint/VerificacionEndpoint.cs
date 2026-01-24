using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using PedidosBarrio.Application.Commands.VerificacionCorreo;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Api.EndPoint
{
    public static class VerificacionEndpoint
    {
        public static void MapVerificacionEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/Verificacion")
                           .WithTags("Verificación de Correo");

            // POST /api/Verificacion/enviar-codigo
            group.MapPost("/enviar-codigo", async ([FromBody] EnviarCodigoVerifDto dto, IMediator mediator) =>
            {
                if (string.IsNullOrEmpty(dto.Correo))
                    return Results.BadRequest("El correo es requerido.");

                var success = await mediator.Send(new SendVerificationCodeCommand(dto.Correo));
                
                return success 
                    ? Results.Ok(new { success = true, message = "Código enviado correctamente." }) 
                    : Results.Problem("Error al enviar el código de verificación.");
            })
            .WithName("SendVerificationCode")
            .WithOpenApi();

            // POST /api/Verificacion/verificar-codigo
            group.MapPost("/verificar-codigo", async ([FromBody] VerificarCodigoDto dto, IMediator mediator) =>
            {
                if (string.IsNullOrEmpty(dto.Correo) || string.IsNullOrEmpty(dto.Codigo))
                    return Results.BadRequest("Correo y código son requeridos.");

                var isValid = await mediator.Send(new VerifyCodeCommand(dto.Correo, dto.Codigo));

                return isValid 
                    ? Results.Ok(new { success = true, message = "Código verificado correctamente." }) 
                    : Results.BadRequest(new { success = false, message = "Código inválido o expirado." });
            })
            .WithName("VerifyCode")
            .WithOpenApi();
        }
    }
}
