namespace PedidosBarrio.Application.DTOs;

/// <summary>
/// DTO para registrar un token de dispositivo
/// </summary>
public class RegisterDeviceTokenDto
{
    /// <summary>
    /// Token FCM del dispositivo
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// ID del cliente (opcional)
    /// </summary>
    public int? ClienteId { get; set; }

    /// <summary>
    /// ID de la empresa (opcional)
    /// </summary>
    public Guid? EmpresaId { get; set; }

    /// <summary>
    /// Plataforma: iOS, Android, Web
    /// </summary>
    public string? Platform { get; set; }

    /// <summary>
    /// ID único del dispositivo
    /// </summary>
    public string? DeviceId { get; set; }
}

/// <summary>
/// DTO para enviar notificación push
/// </summary>
public class SendPushNotificationDto
{
    /// <summary>
    /// Título de la notificación
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Cuerpo de la notificación
    /// </summary>
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de destinatarios: "all", "empresa", "cliente", "token"
    /// </summary>
    public string TargetType { get; set; } = "all"; // all, empresa, cliente, token

    /// <summary>
    /// ID de la empresa (si TargetType es "empresa")
    /// </summary>
    public Guid? EmpresaId { get; set; }

    /// <summary>
    /// ID del cliente (si TargetType es "cliente")
    /// </summary>
    public int? ClienteId { get; set; }

    /// <summary>
    /// Token específico (si TargetType es "token")
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Tópico (si TargetType es "topic")
    /// </summary>
    public string? Topic { get; set; }

    /// <summary>
    /// Datos adicionales para la notificación
    /// </summary>
    public Dictionary<string, string>? Data { get; set; }
}

/// <summary>
/// Respuesta de envío de notificación
/// </summary>
public class PushNotificationResponseDto
{
    /// <summary>
    /// Indicador de éxito
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Mensaje de respuesta
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Número de notificaciones enviadas exitosamente
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Número de notificaciones fallidas
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// ID del mensaje (para envíos simples)
    /// </summary>
    public string? MessageId { get; set; }

    /// <summary>
    /// Detalles de errores
    /// </summary>
    public List<string>? Errors { get; set; }
}
