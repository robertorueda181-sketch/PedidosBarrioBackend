using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.Services;

namespace PedidosBarrio.Api.EndPoint
{
    public static class EmailEndpoint
    {
        public static void MapEmailEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/Email")
                           .WithTags("Email")
                           .RequireAuthorization(); // Con autenticación

            // POST /api/Email/test-welcome - Probar email de bienvenida
            group.MapPost("/test-welcome", async (
                [FromQuery] string email,
                [FromQuery] string userName,
                [FromQuery] string businessName,
                [FromQuery] string businessType,
                IEmailService emailService) =>
            {
                if (string.IsNullOrEmpty(email))
                {
                    return Results.BadRequest(new { error = "Email es requerido" });
                }

                try
                {
                    var result = await emailService.SendWelcomeEmailAsync(
                        email,
                        userName ?? "Usuario de Prueba",
                        businessName ?? "Negocio de Prueba",
                        businessType ?? "NEGOCIO"
                    );

                    if (result)
                    {
                        return Results.Ok(new 
                        { 
                            success = true, 
                            message = $"Email de prueba enviado exitosamente a {email}",
                            sentAt = DateTime.UtcNow
                        });
                    }
                    else
                    {
                        return Results.Json(new 
                        { 
                            success = false, 
                            message = "Error al enviar el email" 
                        }, statusCode: 500);
                    }
                }
                catch (Exception ex)
                {
                    return Results.Json(new 
                    { 
                        success = false, 
                        message = $"Error: {ex.Message}" 
                    }, statusCode: 500);
                }
            })
            .WithName("TestWelcomeEmail")
            .WithOpenApi()
            .WithSummary("🧪 Enviar email de bienvenida de prueba")
            .WithDescription("Endpoint para probar el envío de emails de bienvenida. Requiere autenticación.");
        }
    }
}