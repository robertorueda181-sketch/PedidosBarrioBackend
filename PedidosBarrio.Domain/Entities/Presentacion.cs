using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PedidosBarrio.Domain.Entities;

[Table("Presentacion")]
public partial class Presentacion
{
    public Presentacion()
    {
        Precios = new List<Precio>();
    }

    public Presentacion(string descripcion, Guid empresaId, int productoId)
    {
        Descripcion = descripcion;
        EmpresaID = empresaId;
        ProductoID = productoId;
        Precios = new List<Precio>();
    }

    [Key]
    [Column("PresentacionID")]
    public int PresentacionID { get; set; }

    [Required]
    [StringLength(50)]
    public string Descripcion { get; set; } = null!;

    [Column("EmpresaID")]
    public Guid EmpresaID { get; set; }

    [Column("ProductoID")]
    public int ProductoID { get; set; }

    [ForeignKey("EmpresaID")]
    public virtual Empresa Empresa { get; set; } = null!;

    [ForeignKey("ProductoID")]
    public virtual Producto Producto { get; set; } = null!;

    public virtual ICollection<Precio> Precios { get; set; }
}
