using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PedidosBarrio.Domain.Entities;

public partial class Precio
{
    public Precio() { }

    public Precio(decimal valor, int presentacionId, Guid empresaId, bool principal = false, string? descripcion = null)
    {
        PrecioValor = valor;
        PresentacionID = presentacionId;
        EmpresaID = empresaId;
        Principal = principal;
        Descripcion = descripcion;
    }

    [Key]
    [Column("IdPrecio")]
    public int IdPrecio { get; set; }

    [Column("Precio")]
    [Precision(12, 2)]
    public decimal PrecioValor { get; set; }

    [Column("PresentacionID")]
    public int PresentacionID { get; set; }

    [Column("EmpresaID")]
    public Guid EmpresaID { get; set; }

    [Column("EsPrincipal")]
    public bool Principal { get; set; }

    [NotMapped]
    public bool EsPrincipal { get => Principal; set => Principal = value; }

    [StringLength(50)]
    public string? Descripcion { get; set; }

    [ForeignKey("PresentacionID")]
    public virtual Presentacion Presentacion { get; set; } = null!;

    [ForeignKey("EmpresaID")]
    public virtual Empresa Empresa { get; set; } = null!;
}


