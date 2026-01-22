using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PedidosBarrio.Domain.Entities;

[Index("EmpresaID", Name = "idx_productos_empresaid")]
public partial class Producto
{
    public Producto() { }

    public Producto(Guid empresaID, string nombre, string descripcion)
    {
        EmpresaID = empresaID;
        Nombre = nombre;
        Descripcion = descripcion;
        FechaRegistro = DateTime.Now;
        Activa = true;
        Visible = true;
        Aprobado = true;
    }

    [Key]
    [Column("ProductoID")]
    public int ProductoID { get; set; }

    [NotMapped]
    public string? Imagen { get; set; }

    [Column("EmpresaID")]
    public Guid? EmpresaID { get; set; }

    [StringLength(200)]
    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public int Stock { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public bool Activa { get; set; }

    [Column("CategoriaID")]
    public short? CategoriaID { get; set; }

    public int? StockMinimo { get; set; }

    public bool Inventario { get; set; }

    public bool? Visible { get; set; }

    public bool Aprobado { get; set; }

    [ForeignKey("EmpresaID")]
    [InverseProperty("Productos")]
    public virtual Empresa? Empresa { get; set; }

    [InverseProperty("External")]
    public virtual ICollection<Precio> Precios { get; set; } = new List<Precio>();
}


