namespace PedidosBarrio.Application.Services;

/// <summary>
/// Interfaz para el servicio de mensajería Firebase
/// </summary>
public interface IFirebaseMessagingService
{
    /// <summary>
    /// Envía una notificación a un dispositivo específico
    /// </summary>
    /// <param name="token">Token FCM del dispositivo</param>
    /// <param name="title">Título de la notificación</param>
    /// <param name="body">Cuerpo de la notificación</param>
    /// <param name="data">Datos adicionales (opcional)</param>
    /// <returns>ID del mensaje enviado</returns>
    Task<string> SendNotificationAsync(string token, string title, string body, Dictionary<string, string>? data = null);

    /// <summary>
    /// Envía una notificación a múltiples dispositivos
    /// </summary>
    /// <param name="tokens">Lista de tokens FCM</param>
    /// <param name="title">Título de la notificación</param>
    /// <param name="body">Cuerpo de la notificación</param>
    /// <param name="data">Datos adicionales (opcional)</param>
    /// <returns>Tupla con conteo de exitosas y fallidas</returns>
    Task<(int successCount, int failureCount)> SendNotificationToMultipleAsync(List<string> tokens, string title, string body, Dictionary<string, string>? data = null);

    /// <summary>
    /// Envía una notificación a todos los dispositivos suscritos a un tópico
    /// </summary>
    /// <param name="topic">Nombre del tópico</param>
    /// <param name="title">Título de la notificación</param>
    /// <param name="body">Cuerpo de la notificación</param>
    /// <param name="data">Datos adicionales (opcional)</param>
    /// <returns>ID del mensaje enviado</returns>
    Task<string> SendNotificationToTopicAsync(string topic, string title, string body, Dictionary<string, string>? data = null);

    /// <summary>
    /// Suscribe dispositivos a un tópico
    /// </summary>
    /// <param name="tokens">Lista de tokens FCM</param>
    /// <param name="topic">Nombre del tópico</param>
    /// <returns>true si se completó exitosamente</returns>
    Task<bool> SubscribeToTopicAsync(List<string> tokens, string topic);

    /// <summary>
    /// Desuscribe dispositivos de un tópico
    /// </summary>
    /// <param name="tokens">Lista de tokens FCM</param>
    /// <param name="topic">Nombre del tópico</param>
    /// <returns>true si se completó exitosamente</returns>
    Task<bool> UnsubscribeFromTopicAsync(List<string> tokens, string topic);
}
