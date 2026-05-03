
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedidosBarrio.Domain.Entities;

/// <summary>
/// Representa una opción de presentación (ej: Talla M, Color Rojo)
/// </summary>
[Table("PresentacionOpcion")]
public partial class PresentacionOpcion
{
    public PresentacionOpcion() { }

    public PresentacionOpcion(string valor, int presentacionId, decimal? precio = null, string? imagen = null)
    {
        Valor = valor;
        PresentacionID = presentacionId;
        Precio = precio;
        Imagen = imagen;
    }

    [Key]
    [Column("PresentacionOpcionID")]
    public int PresentacionOpcionID { get; set; }

    /// <summary>
    /// Valor de la opción (ej: "M", "Rojo", "Grande")
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Valor { get; set; } = null!;

    [Column("PresentacionID")]
    public int PresentacionID { get; set; }

    /// <summary>
    /// Precio específico de esta opción (si es diferente al precio principal)
    /// </summary>
    [Column("Precio")]
    [Precision(12, 2)]
    public decimal? Precio { get; set; }

    /// <summary>
    /// URL de imagen específica para esta opción
    /// </summary>
    [StringLength(500)]
    public string? Imagen { get; set; }

    /// <summary>
    /// Descripción adicional de la opción
    /// </summary>
    [StringLength(255)]
    public string? Descripcion { get; set; }

    /// <summary>
    /// Si está activa
    /// </summary>
    public bool Activa { get; set; } = true;

    /// <summary>
    /// Stock disponible para esta opción específica
    /// </summary>
    public int? Stock { get; set; }

    [ForeignKey("PresentacionID")]
    public virtual Presentacion Presentacion { get; set; } = null!;
}
