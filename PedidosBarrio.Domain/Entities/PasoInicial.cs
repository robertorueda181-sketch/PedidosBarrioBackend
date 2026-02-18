using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedidosBarrio.Domain.Entities;

[Table("PasosIniciales")]
public class PasoInicial
{
    [Key]
    [Column("PasoID")]
    public int PasoID { get; set; }

    [Column("EmpresaID")]
    public Guid EmpresaID { get; set; }

    [Column("Codigo")]
    [StringLength(50)]
    public string? Codigo { get; set; }

    [Column("Titulo")]
    [StringLength(255)]
    public string? Titulo { get; set; }

    [Column("Descripcion")]
    [StringLength(500)]
    public string? Descripcion { get; set; }

    [Column("Icono")]
    [StringLength(100)]
    public string? Icono { get; set; }

    [Column("Ruta")]
    [StringLength(255)]
    public string? Ruta { get; set; }

    [Column("Obligatorio")]
    public bool Obligatorio { get; set; } = true;

    [Column("Completado")]
    public bool Completado { get; set; } = false;

    [Column("Orden")]
    public int Orden { get; set; }

    [Column("FechaCreacion")]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    [Column("FechaCompletado")]
    public DateTime? FechaCompletado { get; set; }

    [Column("Activo")]
    public bool Activo { get; set; } = true;
}
