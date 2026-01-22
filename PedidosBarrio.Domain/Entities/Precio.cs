using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PedidosBarrio.Domain.Entities;

public partial class Precio
{
    public Precio() { }

    public Precio(int idPrecio, int productoId, Guid empresaId, string descripcion, string tipo, decimal valor, bool principal)
    {
        IdPrecio = idPrecio;
        ExternalId = productoId;
        EmpresaID = empresaId;
        PrecioValor = valor;
        Principal = principal;
        Tipo = tipo;
        Descripcion = descripcion;
        FechaCreacion = DateTime.Now;
        Activo = true;
    }

    public Precio(decimal valor, int productoId, Guid empresaId)
    {
        PrecioValor = valor;
        ExternalId = productoId;
        EmpresaID = empresaId;
        Principal = true;
        FechaCreacion = DateTime.Now;
        Activo = true;
    }

    [Key]
    public int IdPrecio { get; set; }

    [NotMapped]
    public bool EsPrincipal { get => Principal; set => Principal = value; }

    [NotMapped]
    public bool Activo { get; set; } = true;

    [NotMapped]
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    [NotMapped]
    public string? Tipo { get; set; }

    [NotMapped]
    public string? Descripcion { get; set; }

    [Column("Precio")]
    [Precision(12, 2)]
    public decimal PrecioValor { get; set; }

    public int ExternalId { get; set; }

    [Column("EmpresaID")]
    public Guid EmpresaID { get; set; }

    public bool Principal { get; set; }

    [ForeignKey("ExternalId")]
    [InverseProperty("Precios")]
    public virtual Producto External { get; set; } = null!;
}


