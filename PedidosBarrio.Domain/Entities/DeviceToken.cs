using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedidosBarrio.Domain.Entities;

/// <summary>
/// Representa un token de dispositivo registrado para recibir notificaciones push
/// </summary>
[Table("DeviceTokens")]
public class DeviceToken
{
    [Key]
    [Column("DeviceTokenID")]
    public int Id { get; set; }

    /// <summary>
    /// Token único del dispositivo para Firebase
    /// </summary>
    [Column("Token")]
    [StringLength(500)]
    public string Token { get; set; } = null!;

    /// <summary>
    /// ID del cliente/usuario asociado (opcional)
    /// </summary>
    [Column("ClienteID")]
    public int? ClienteId { get; set; }

    /// <summary>
    /// ID de la empresa (opcional, para filtrar por empresa)
    /// </summary>
    [Column("EmpresaID")]
    public Guid? EmpresaId { get; set; }

    /// <summary>
    /// Plataforma del dispositivo (iOS, Android, Web)
    /// </summary>
    [Column("Platform")]
    [StringLength(50)]
    public string? Platform { get; set; }

    /// <summary>
    /// Identificador único del dispositivo
    /// </summary>
    [Column("DeviceId")]
    [StringLength(255)]
    public string? DeviceId { get; set; }

    /// <summary>
    /// Indica si el token está activo
    /// </summary>
    [Column("IsActive")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Fecha de registro del token
    /// </summary>
    [Column("RegisteredDate")]
    public DateTime RegisteredDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Última vez que se validó el token
    /// </summary>
    [Column("LastUsedDate")]
    public DateTime? LastUsedDate { get; set; }
}
