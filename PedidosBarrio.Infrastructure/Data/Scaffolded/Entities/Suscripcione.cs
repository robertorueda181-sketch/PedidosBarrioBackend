using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PedidosBarrio.Infrastructure.Data.Scaffolded.Entities;

[Index("EmpresaId", Name = "idx_suscripciones_empresaid")]
public partial class Suscripcione
{
    [Key]
    [Column("SuscripcionID")]
    public int SuscripcionId { get; set; }

    [Column("EmpresaID")]
    public Guid? EmpresaId { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    [Precision(10, 2)]
    public decimal? Monto { get; set; }

    public bool? Activa { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public short? NivelSuscripcion { get; set; }

    [ForeignKey("EmpresaId")]
    [InverseProperty("Suscripciones")]
    public virtual Empresa? Empresa { get; set; }
}
