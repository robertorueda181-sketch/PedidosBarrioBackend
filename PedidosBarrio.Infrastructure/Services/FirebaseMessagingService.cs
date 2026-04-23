using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Services;

namespace PedidosBarrio.Infrastructure.Services;

/// <summary>
/// Servicio para enviar notificaciones push usando Firebase Cloud Messaging
/// </summary>
public class FirebaseMessagingService : IFirebaseMessagingService
{
    private readonly IApplicationLogger _logger;
    private readonly FirebaseApp? _firebaseApp;

    public FirebaseMessagingService(IApplicationLogger logger, IWebHostEnvironment environment, IConfiguration configuration)
    {
        _logger = logger;

        try
        {
            // Inicializar Firebase si no está inicializado
            if (FirebaseApp.DefaultInstance == null)
            {
                var serviceAccountPath = configuration["Firebase:ServiceAccountPath"];

                if (string.IsNullOrEmpty(serviceAccountPath))
                {
                    var infrastructurePath = Path.Combine(environment.ContentRootPath, "messagesespacioonline-firebase-adminsdk-fbsvc-3497fea4a0.json");
                    if (File.Exists(infrastructurePath))
                    {
                        serviceAccountPath = infrastructurePath;
                    }
                    else
                    {
                        throw new FileNotFoundException("Firebase service account JSON no encontrado");
                    }
                }

                var credential = GoogleCredential.FromFile(serviceAccountPath);
                _firebaseApp = FirebaseApp.Create(new AppOptions()
                {
                    Credential = credential,
                }, "PedidosBarrio");

                _logger.LogInformationAsync("Firebase inicializado exitosamente", "FirebaseMessagingService");
            }
            else
            {
                _firebaseApp = FirebaseApp.DefaultInstance;
            }
        }
        catch (Exception ex)
        {
            _logger.LogErrorAsync($"Error al inicializar Firebase: {ex.Message}", ex, "FirebaseMessagingService");
            throw;
        }
    }

    public async Task<string> SendNotificationAsync(string token, string title, string body, Dictionary<string, string>? data = null)
    {
        try
        {
            var message = new Message()
            {
                Token = token,
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                },
                Data = data ?? new Dictionary<string, string>()
            };

            var messaging = FirebaseMessaging.GetMessaging(_firebaseApp);
            var messageId = await messaging.SendAsync(message);

            await _logger.LogInformationAsync($"Notificación enviada exitosamente: {messageId}", "FirebaseMessagingService");
            return messageId;
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync($"Error al enviar notificación: {ex.Message}", ex, "FirebaseMessagingService");
            throw;
        }
    }

    public async Task<(int successCount, int failureCount)> SendNotificationToMultipleAsync(List<string> tokens, string title, string body, Dictionary<string, string>? data = null)
    {
        try
        {
            if (!tokens.Any())
            {
                throw new ArgumentException("La lista de tokens no puede estar vacía");
            }

            var message = new MulticastMessage()
            {
                Tokens = tokens,
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                },
                Data = data ?? new Dictionary<string, string>()
            };

            var messaging = FirebaseMessaging.GetMessaging(_firebaseApp);
            var response = await messaging.SendMulticastAsync(message);

            await _logger.LogInformationAsync(
                $"Notificaciones masivas enviadas: {response.SuccessCount} exitosas, {response.FailureCount} fallidas",
                "FirebaseMessagingService");

            return (response.SuccessCount, response.FailureCount);
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync($"Error al enviar notificaciones masivas: {ex.Message}", ex, "FirebaseMessagingService");
            throw;
        }
    }

    public async Task<string> SendNotificationToTopicAsync(string topic, string title, string body, Dictionary<string, string>? data = null)
    {
        try
        {
            var message = new Message()
            {
                Topic = topic,
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                },
                Data = data ?? new Dictionary<string, string>()
            };

            var messaging = FirebaseMessaging.GetMessaging(_firebaseApp);
            var messageId = await messaging.SendAsync(message);

            await _logger.LogInformationAsync($"Notificación por tópico enviada: {messageId}", "FirebaseMessagingService");
            return messageId;
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync($"Error al enviar notificación por tópico: {ex.Message}", ex, "FirebaseMessagingService");
            throw;
        }
    }

    public async Task<bool> SubscribeToTopicAsync(List<string> tokens, string topic)
    {
        try
        {
            if (!tokens.Any())
            {
                throw new ArgumentException("La lista de tokens no puede estar vacía");
            }

            var messaging = FirebaseMessaging.GetMessaging(_firebaseApp);
            await messaging.SubscribeToTopicAsync(tokens, topic);

            await _logger.LogInformationAsync($"Dispositivos suscritos al tópico '{topic}': {tokens.Count}", "FirebaseMessagingService");
            return true;
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync($"Error al suscribir dispositivos al tópico: {ex.Message}", ex, "FirebaseMessagingService");
            throw;
        }
    }

    public async Task<bool> UnsubscribeFromTopicAsync(List<string> tokens, string topic)
    {
        try
        {
            if (!tokens.Any())
            {
                throw new ArgumentException("La lista de tokens no puede estar vacía");
            }

            var messaging = FirebaseMessaging.GetMessaging(_firebaseApp);
            await messaging.UnsubscribeFromTopicAsync(tokens, topic);

            await _logger.LogInformationAsync($"Dispositivos desuscritos del tópico '{topic}': {tokens.Count}", "FirebaseMessagingService");
            return true;
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync($"Error al desuscribir dispositivos del tópico: {ex.Message}", ex, "FirebaseMessagingService");
            throw;
        }
    }
}
