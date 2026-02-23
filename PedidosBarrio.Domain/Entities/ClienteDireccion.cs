using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedidosBarrio.Domain.Entities;

/// <summary>
/// Representa las direcciones de un cliente
/// Un cliente puede tener múltiples direcciones (casa, trabajo, etc.)
/// </summary>
[Table("ClienteDirecciones")]
public partial class ClienteDireccion
{
    public ClienteDireccion() { }

    public ClienteDireccion(
        Guid clienteId,
        string nombre,
        string direccionTexto,
        decimal latitud,
        decimal longitud)
    {
        ClienteID = clienteId;
        Nombre = nombre;
        DireccionTexto = direccionTexto;
        Latitud = latitud;
        Longitud = longitud;
        EsPrincipal = false;
        FechaCreacion = DateTime.UtcNow;
        Activa = true;
    }

    [Key]
    [Column("ClienteDireccionID")]
    public Guid ClienteDireccionID { get; set; } = Guid.NewGuid();

    [Column("ClienteID")]
    public Guid ClienteID { get; set; }

    /// <summary>
    /// Nombre de la dirección (ej: Casa, Trabajo, Departamento, etc.)
    /// </summary>
    [Required]
    [StringLength(100)]
    [Column("Nombre")]
    public string Nombre { get; set; } = null!;

    /// <summary>
    /// Texto completo de la dirección (Calle, número, etc.)
    /// </summary>
    [Required]
    [StringLength(500)]
    [Column("DireccionTexto")]
    public string DireccionTexto { get; set; } = null!;

    /// <summary>
    /// Referencia adicional (ej: Apt 505, después del parque, etc.)
    /// </summary>
    [StringLength(255)]
    public string? Referencia { get; set; }

    /// <summary>
    /// Coordenada de latitud
    /// </summary>
    [Column(TypeName = "numeric(9,6)")]
    public decimal Latitud { get; set; }

    /// <summary>
    /// Coordenada de longitud
    /// </summary>
    [Column(TypeName = "numeric(9,6)")]
    public decimal Longitud { get; set; }

    /// <summary>
    /// Departamento/Región administrativo
    /// </summary>
    [StringLength(100)]
    public string? Departamento { get; set; }

    /// <summary>
    /// Provincia
    /// </summary>
    [StringLength(100)]
    public string? Provincia { get; set; }

    /// <summary>
    /// Distrito
    /// </summary>
    [StringLength(100)]
    public string? Distrito { get; set; }

    /// <summary>
    /// Código postal
    /// </summary>
    [StringLength(20)]
    public string? CodigoPostal { get; set; }

    /// <summary>
    /// Indica si es la dirección principal del cliente
    /// </summary>
    public bool EsPrincipal { get; set; } = false;

    /// <summary>
    /// Indica si la dirección está activa
    /// </summary>
    public bool Activa { get; set; } = true;

    /// <summary>
    /// Fecha de creación de la dirección
    /// </summary>
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha de última actualización
    /// </summary>
    public DateTime? FechaActualizacion { get; set; }

    // Foreign Key
    [ForeignKey("ClienteID")]
    [InverseProperty("Direcciones")]
    public virtual Cliente? Cliente { get; set; }
}
