using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedidosBarrio.Domain.Entities;

[Table("PageViews")]
public partial class PageView
{
    [Key]
    [Column("PageViewID")]
    public int PageViewID { get; set; }

    [Column("EmpresaID")]
    public Guid EmpresaID { get; set; }

    [Column("Url")]
    [StringLength(1000)]
    public string Url { get; set; } = null!;

    [Column("Fecha")]
    public DateTime Fecha { get; set; }

    [Column("UserAgent")]
    [StringLength(500)]
    public string? UserAgent { get; set; }

    [Column("IpAddress")]
    [StringLength(45)]
    public string? IpAddress { get; set; }

    [Column("Referrer")]
    [StringLength(1000)]
    public string? Referrer { get; set; }

    [Column("Processed")]
    public bool Processed { get; set; } = false;

    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("ProcessedAt")]
    public DateTime? ProcessedAt { get; set; }

    [ForeignKey("EmpresaID")]
    [InverseProperty("PageViews")]
    public virtual Empresa? Empresa { get; set; }
}
