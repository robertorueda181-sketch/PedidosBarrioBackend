using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PedidosBarrio.Infrastructure.Data.Scaffolded.Entities;

public partial class Empresa
{
    [Key]
    [Column("EmpresaID")]
    public Guid EmpresaId { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public bool? Activa { get; set; }

    [Column("UsuarioID")]
    public Guid? UsuarioId { get; set; }

    public short? TipoEmpresa { get; set; }

    public bool Visible { get; set; }

    public bool Aprobado { get; set; }

    [InverseProperty("Empresa")]
    public virtual ICollection<Inmueble> Inmuebles { get; set; } = new List<Inmueble>();

    [InverseProperty("Empresa")]
    public virtual ICollection<Negocio> Negocios { get; set; } = new List<Negocio>();

    [InverseProperty("Empresa")]
    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();

    [InverseProperty("Empresa")]
    public virtual ICollection<Suscripcione> Suscripciones { get; set; } = new List<Suscripcione>();
}
