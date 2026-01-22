using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PedidosBarrio.Infrastructure.Data.Scaffolded.Entities;

[Index("EmpresaId", Name = "idx_inmuebles_empresaid")]
[Index("TiposId", Name = "idx_inmuebles_tiposid")]
public partial class Inmueble
{
    [Key]
    [Column("InmuebleID")]
    public int InmuebleId { get; set; }

    [Column("EmpresaID")]
    public Guid? EmpresaId { get; set; }

    [Column("TiposID")]
    public int? TiposId { get; set; }

    [Precision(10, 2)]
    public decimal? Precio { get; set; }

    [StringLength(100)]
    public string? Medidas { get; set; }

    [StringLength(255)]
    public string? Ubicacion { get; set; }

    public int? Dormitorios { get; set; }

    public int? Banos { get; set; }

    public string? Descripcion { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public bool? Activa { get; set; }

    public short? Operacion { get; set; }

    [Column("latitud")]
    [Precision(10, 8)]
    public decimal? Latitud { get; set; }

    [Column("longitud")]
    [Precision(11, 8)]
    public decimal? Longitud { get; set; }

    [ForeignKey("EmpresaId")]
    [InverseProperty("Inmuebles")]
    public virtual Empresa? Empresa { get; set; }

    [ForeignKey("TiposId")]
    [InverseProperty("Inmuebles")]
    public virtual Tipo? Tipos { get; set; }
}
