using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PedidosBarrio.Domain.Entities;

public partial class Tipo
{
    [Key]
    [Column("TipoID")]
    public int TipoID { get; set; }

    [Column("Tipo")]
    [StringLength(100)]
    public string Tipo1 { get; set; } = null!;

    [NotMapped]
    public string? Descripcion { get => Tipo1; set => Tipo1 = value!; }

    [StringLength(100)]
    public string? Parametro { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public bool? Activa { get; set; }

    [StringLength(10)]
    public string? Codigo { get; set; }

    [StringLength(15)]
    public string? Icono { get; set; }

    [InverseProperty("Tipos")]
    public virtual ICollection<Inmueble> Inmuebles { get; set; } = new List<Inmueble>();

    [InverseProperty("Tipos")]
    public virtual ICollection<Negocio> Negocios { get; set; } = new List<Negocio>();
}


