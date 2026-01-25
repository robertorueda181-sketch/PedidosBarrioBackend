using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PedidosBarrio.Domain.Entities;

[Index("EmpresaID", Name = "idx_inmuebles_empresaid")]
[Index("TiposId", Name = "idx_inmuebles_tiposid")]
public partial class Inmueble
{
    public Inmueble() { }

    public Inmueble(Guid empresaID, int tiposID, decimal precio, string medidas, string ubicacion, int dormitorios, int banos, string descripcion)
    {
        EmpresaID = empresaID;
        TiposId = tiposID;
        Precio = precio;
        Medidas = medidas;
        Ubicacion = ubicacion;
        Dormitorios = dormitorios;
        Banos = banos;
        Descripcion = descripcion;
        FechaRegistro = DateTime.UtcNow;
        Activa = true;
    }

    [Key]
    [Column("InmuebleID")]
    public int InmuebleID { get; set; }

    [Column("EmpresaID")]
    public Guid? EmpresaID { get; set; }

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

    [NotMapped]
    public string? Tipo { get; set; }

    [NotMapped]
    public Imagen? Imagen { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public bool? Activa { get; set; }

    [Column("Operacion")]
    public int? OperacionID { get; set; }

    [ForeignKey("OperacionID")]
    public virtual Tipo? Operacion { get; set; }

    [Column("latitud")]
    [Precision(10, 8)]
    public decimal? Latitud { get; set; }

    [Column("longitud")]
    [Precision(11, 8)]
    public decimal? Longitud { get; set; }

    [ForeignKey("EmpresaID")]
    [InverseProperty("Inmuebles")]
    public virtual Empresa? Empresa { get; set; }

    [ForeignKey("TiposId")]
    [InverseProperty("Inmuebles")]
    public virtual Tipo? Tipos { get; set; }
}


