using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace PedidosBarrio.Domain.Entities;

[Index("EmpresaID", Name = "idx_negocios_empresaid")]
[Index("TiposId", Name = "idx_negocios_tiposid")]
public partial class Negocio
{
    public Negocio() { }

    public Negocio(Guid empresaID, int tiposID, string urlNegocio, string descripcion, string urlOpcional = null)
    {
        EmpresaID = empresaID;
        TiposId = tiposID;
        Urlnegocio = urlNegocio;
        Descripcion = descripcion;
        Urlopcional = urlOpcional;
        FechaRegistro = DateTime.UtcNow;
        Activa = true;
    }

    [Key]
    [Column("NegocioID")]
    public int NegocioID { get; set; }

    [Column("EmpresaID")]
    public Guid? EmpresaID { get; set; }

    [Column("TiposID")]
    public int? TiposId { get; set; }

    [NotMapped]
    public int TiposID { get => TiposId ?? 0; set => TiposId = value; }

    [Column("URLNegocio")]
    [StringLength(500)]
    public string? Urlnegocio { get; set; }

    [NotMapped]
    public string? URLNegocio { get => Urlnegocio; set => Urlnegocio = value; }

    [Column("URLOpcional")]
    [StringLength(500)]
    public string? Urlopcional { get; set; }

    [NotMapped]
    public string? URLOpcional { get => Urlopcional; set => Urlopcional = value; }

    public string? Descripcion { get; set; }

    [NotMapped]
    public Imagen? Imagenes { get; set; }

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

    [ForeignKey("EmpresaID")]
    [InverseProperty("Negocios")]
    public virtual Empresa? Empresa { get; set; }

    [ForeignKey("TiposId")]
    [InverseProperty("Negocios")]
    public virtual Tipo? Tipos { get; set; }
}


