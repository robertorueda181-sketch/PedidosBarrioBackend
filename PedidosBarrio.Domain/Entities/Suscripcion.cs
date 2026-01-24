using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PedidosBarrio.Domain.Entities;

[Index("EmpresaID", Name = "idx_Suscripcions_empresaid")]
public partial class Suscripcion
{
    public Suscripcion() { }

    public Suscripcion(Guid empresaID, decimal monto, DateTime? fechaFin)
    {
        EmpresaID = empresaID;
        Monto = monto;
        FechaFin = fechaFin?.Kind == DateTimeKind.Unspecified 
            ? DateTime.SpecifyKind(fechaFin.Value, DateTimeKind.Utc) 
            : fechaFin?.ToUniversalTime();
        FechaInicio = DateTime.UtcNow;
        Activa = true;
        FechaRegistro = DateTime.UtcNow;
    }

    [Key]
    [Column("SuscripcionID")]
    public int SuscripcionID { get; set; }

    [Column("EmpresaID")]
    public Guid? EmpresaID { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    [Precision(10, 2)]
    public decimal? Monto { get; set; }

    public bool? Activa { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public short? NivelSuscripcion { get; set; }

    [ForeignKey("EmpresaID")]
    [InverseProperty("Suscripcions")]
    public virtual Empresa? Empresa { get; set; }
}


