using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace PedidosBarrio.Infrastructure.Data.Scaffolded.Entities;

[Index("EmpresaId", Name = "idx_negocios_empresaid")]
[Index("TiposId", Name = "idx_negocios_tiposid")]
public partial class Negocio
{
    [Key]
    [Column("NegocioID")]
    public int NegocioId { get; set; }

    [Column("EmpresaID")]
    public Guid? EmpresaId { get; set; }

    [Column("TiposID")]
    public int? TiposId { get; set; }

    [Column("URLNegocio")]
    [StringLength(500)]
    public string? Urlnegocio { get; set; }

    [Column("URLOpcional")]
    [StringLength(500)]
    public string? Urlopcional { get; set; }

    public string? Descripcion { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public bool? Activa { get; set; }

    [StringLength(500)]
    public string? BaseUrl { get; set; }

    [StringLength(50)]
    public string? Nombre { get; set; }

    [StringLength(100)]
    public string? Referencia { get; set; }

    [Column("longitud")]
    public NpgsqlPoint? Longitud { get; set; }

    [Column("latitud")]
    public NpgsqlPoint? Latitud { get; set; }

    [StringLength(100)]
    public string? Direccion { get; set; }

    [StringLength(12)]
    public string? Telefono { get; set; }

    [StringLength(20)]
    public string? Codigo { get; set; }

    [ForeignKey("EmpresaId")]
    [InverseProperty("Negocios")]
    public virtual Empresa? Empresa { get; set; }

    [ForeignKey("TiposId")]
    [InverseProperty("Negocios")]
    public virtual Tipo? Tipos { get; set; }
}
