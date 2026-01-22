using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PedidosBarrio.Infrastructure.Data.Scaffolded.Entities;

[Index("EmpresaId", Name = "idx_productos_empresaid")]
public partial class Producto
{
    [Key]
    [Column("ProductoID")]
    public int ProductoId { get; set; }

    [Column("EmpresaID")]
    public Guid? EmpresaId { get; set; }

    [StringLength(200)]
    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public int Stock { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public bool? Activa { get; set; }

    [Column("CategoriaID")]
    public short? CategoriaId { get; set; }

    public int? StockMinimo { get; set; }

    public bool Inventario { get; set; }

    public bool Visible { get; set; }

    public bool Aprobado { get; set; }

    [ForeignKey("EmpresaId")]
    [InverseProperty("Productos")]
    public virtual Empresa? Empresa { get; set; }

    [InverseProperty("External")]
    public virtual ICollection<Precio> Precios { get; set; } = new List<Precio>();
}
