using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PedidosBarrio.Infrastructure.Data.Scaffolded.Entities;

public partial class Precio
{
    [Key]
    public int IdPrecio { get; set; }

    [Column("Precio")]
    [Precision(12, 2)]
    public decimal Precio1 { get; set; }

    public int ExternalId { get; set; }

    [Column("EmpresaID")]
    public Guid EmpresaId { get; set; }

    public bool Principal { get; set; }

    [ForeignKey("ExternalId")]
    [InverseProperty("Precios")]
    public virtual Producto External { get; set; } = null!;
}
