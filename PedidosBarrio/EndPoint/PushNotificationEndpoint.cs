using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Api.EndPoint;

/// <summary>
/// Endpoint para gestionar notificaciones push a través de Firebase
/// </summary>
public static class PushNotificationEndpoint
{
    public static void MapPushNotificationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/Notificaciones/Push")
                       .WithTags("Push Notifications");

        // POST /api/Notificaciones/Push/registrar - Registrar token de dispositivo
        group.MapPost("/registrar", RegisterDeviceToken)
            .WithName("RegisterDeviceToken")
            .WithOpenApi()
            .AllowAnonymous()
            .Produces<PushNotificationResponseDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithSummary("📱 Registrar token de dispositivo")
            .WithDescription("Registra el token FCM de un dispositivo para recibir notificaciones push.");

        // POST /api/Notificaciones/Push/enviar - Enviar notificación masiva
        group.MapPost("/enviar", SendPushNotification)
            .WithName("SendPushNotification")
            .WithOpenApi()
            .RequireAuthorization()
            .Produces<PushNotificationResponseDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .WithSummary("🔔 Enviar notificación push masiva")
            .WithDescription("Envía una notificación a todos los dispositivos registrados, a una empresa específica, o a un cliente específico.");

        // POST /api/Notificaciones/Push/desuscribir - Desuscribir token
        group.MapPost("/desuscribir", UnregisterDeviceToken)
            .WithName("UnregisterDeviceToken")
            .WithOpenApi()
            .AllowAnonymous()
            .Produces<PushNotificationResponseDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithSummary("🚫 Desuscribir token de dispositivo")
            .WithDescription("Desactiva un token de dispositivo para que ya no reciba notificaciones.");

        // GET /api/Notificaciones/Push/estado/{token} - Verificar estado del token
        group.MapGet("/estado/{token}", CheckTokenStatus)
            .WithName("CheckTokenStatus")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("🔍 Verificar estado del token")
            .WithDescription("Verifica si un token está registrado y activo.");
    }

    private static async Task<IResult> RegisterDeviceToken(
        [FromBody] RegisterDeviceTokenDto request,
        IDeviceTokenRepository deviceTokenRepository)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Token))
            {
                return Results.BadRequest(new PushNotificationResponseDto
                {
                    Success = false,
                    Message = "El token de dispositivo es requerido"
                });
            }

            // Verificar si el token ya existe
            var exists = await deviceTokenRepository.ExistsAsync(request.Token);
            if (exists)
            {
                return Results.Ok(new PushNotificationResponseDto
                {
                    Success = true,
                    Message = "Token ya registrado anteriormente"
                });
            }

            // Registrar nuevo token
            var deviceToken = new PedidosBarrio.Domain.Entities.DeviceToken
            {
                Token = request.Token,
                ClienteId = request.ClienteId,
                EmpresaId = request.EmpresaId,
                Platform = request.Platform ?? "Web",
                DeviceId = request.DeviceId,
                IsActive = true,
                RegisteredDate = DateTime.UtcNow
            };

            var id = await deviceTokenRepository.AddAsync(deviceToken);

            return Results.Ok(new PushNotificationResponseDto
            {
                Success = true,
                Message = $"Token registrado exitosamente (ID: {id})"
            });
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: $"Error al registrar token: {ex.Message}",
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Error en registro de token"
            );
        }
    }

    private static async Task<IResult> SendPushNotification(
        [FromBody] SendPushNotificationDto request,
        IFirebaseMessagingService firebaseService,
        IDeviceTokenRepository deviceTokenRepository)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Body))
            {
                return Results.BadRequest(new PushNotificationResponseDto
                {
                    Success = false,
                    Message = "El título y cuerpo de la notificación son requeridos"
                });
            }

            List<string> tokens = new();

            // Obtener tokens según el tipo de destino
            switch (request.TargetType?.ToLower())
            {
                case "all":
                    tokens = await deviceTokenRepository.GetActiveTokensAsync(0, 500);
                    break;

                case "empresa":
                    if (!request.EmpresaId.HasValue)
                        return Results.BadRequest(new PushNotificationResponseDto
                        {
                            Success = false,
                            Message = "EmpresaId es requerido para TargetType 'empresa'"
                        });
                    var empresaTokens = await deviceTokenRepository.GetActiveByEmpresaAsync(request.EmpresaId.Value);
                    tokens = empresaTokens.Select(t => t.Token).ToList();
                    break;

                case "cliente":
                    if (!request.ClienteId.HasValue)
                        return Results.BadRequest(new PushNotificationResponseDto
                        {
                            Success = false,
                            Message = "ClienteId es requerido para TargetType 'cliente'"
                        });
                    var clienteTokens = await deviceTokenRepository.GetActiveByClienteAsync(request.ClienteId.Value);
                    tokens = clienteTokens.Select(t => t.Token).ToList();
                    break;

                case "token":
                    if (string.IsNullOrWhiteSpace(request.Token))
                        return Results.BadRequest(new PushNotificationResponseDto
                        {
                            Success = false,
                            Message = "Token es requerido para TargetType 'token'"
                        });
                    tokens.Add(request.Token);
                    break;

                case "topic":
                    if (string.IsNullOrWhiteSpace(request.Topic))
                        return Results.BadRequest(new PushNotificationResponseDto
                        {
                            Success = false,
                            Message = "Topic es requerido para TargetType 'topic'"
                        });
                    // Enviar al tópico
                    var messageId = await firebaseService.SendNotificationToTopicAsync(
                        request.Topic,
                        request.Title,
                        request.Body,
                        request.Data
                    );
                    return Results.Ok(new PushNotificationResponseDto
                    {
                        Success = true,
                        Message = "Notificación enviada al tópico",
                        MessageId = messageId
                    });

                default:
                    return Results.BadRequest(new PushNotificationResponseDto
                    {
                        Success = false,
                        Message = "TargetType inválido. Valores permitidos: all, empresa, cliente, token, topic"
                    });
            }

            // Validar que hay tokens
            if (!tokens.Any())
            {
                return Results.Ok(new PushNotificationResponseDto
                {
                    Success = true,
                    Message = "No hay dispositivos registrados para enviar notificaciones",
                    SuccessCount = 0,
                    FailureCount = 0
                });
            }

            // Enviar notificaciones en lotes (Firebase tiene límite de 500 tokens por request)
            int totalSuccess = 0;
            int totalFailure = 0;
            var errors = new List<string>();

            for (int i = 0; i < tokens.Count; i += 500)
            {
                var batch = tokens.Skip(i).Take(500).ToList();
                var (successCount, failureCount) = await firebaseService.SendNotificationToMultipleAsync(
                    batch,
                    request.Title,
                    request.Body,
                    request.Data
                );

                totalSuccess += successCount;
                totalFailure += failureCount;
            }

            return Results.Ok(new PushNotificationResponseDto
            {
                Success = true,
                Message = "Notificaciones enviadas",
                SuccessCount = totalSuccess,
                FailureCount = totalFailure
            });
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: $"Error al enviar notificación: {ex.Message}",
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Error en envío de notificación"
            );
        }
    }

    private static async Task<IResult> UnregisterDeviceToken(
        [FromBody] UnregisterTokenRequest request,
        IDeviceTokenRepository deviceTokenRepository)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Token))
            {
                return Results.BadRequest(new PushNotificationResponseDto
                {
                    Success = false,
                    Message = "El token es requerido"
                });
            }

            var result = await deviceTokenRepository.DeactivateByTokenAsync(request.Token);

            return Results.Ok(new PushNotificationResponseDto
            {
                Success = result,
                Message = result ? "Token desuscrito exitosamente" : "Token no encontrado"
            });
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: $"Error al desuscribir token: {ex.Message}",
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Error en desuscripción"
            );
        }
    }

    private static async Task<IResult> CheckTokenStatus(
        string token,
        IDeviceTokenRepository deviceTokenRepository)
    {
        try
        {
            var deviceToken = await deviceTokenRepository.GetByTokenAsync(token);

            if (deviceToken == null)
            {
                return Results.NotFound(new { message = "Token no registrado" });
            }

            return Results.Ok(new
            {
                isActive = deviceToken.IsActive,
                platform = deviceToken.Platform,
                registeredDate = deviceToken.RegisteredDate,
                lastUsedDate = deviceToken.LastUsedDate,
                clienteId = deviceToken.ClienteId,
                empresaId = deviceToken.EmpresaId
            });
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: $"Error al verificar token: {ex.Message}",
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Error en verificación"
            );
        }
    }
}

public class UnregisterTokenRequest
{
    public string Token { get; set; } = string.Empty;
}
