using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedidosBarrio.Domain.Entities;

[Table("Presentacion")]
public partial class Presentacion
{
    public Presentacion()
    {
        Opciones = new List<PresentacionOpcion>();
    }

    public Presentacion(string descripcion, Guid empresaId, int productoId)
    {
        Descripcion = descripcion;
        EmpresaID = empresaId;
        ProductoID = productoId;
        Opciones = new List<PresentacionOpcion>();
    }

    [Key]
    [Column("PresentacionID")]
    public int PresentacionID { get; set; }

    /// <summary>
    /// Nombre de la presentación (ej: "Talla", "Color", "Tamaño")
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Descripcion { get; set; } = null!;

    [Column("EmpresaID")]
    public Guid EmpresaID { get; set; }

    [Column("ProductoID")]
    public int ProductoID { get; set; }

    /// <summary>
    /// Si esta presentación está activa
    /// </summary>
    public bool Activa { get; set; } = true;

    [ForeignKey("EmpresaID")]
    public virtual Empresa Empresa { get; set; } = null!;

    [ForeignKey("ProductoID")]
    public virtual Producto Producto { get; set; } = null!;
    /// <summary>
    /// Opciones de esta presentación (ej: S, M, L para Talla)
    /// </summary>
    public virtual ICollection<PresentacionOpcion> Opciones { get; set; }
}
