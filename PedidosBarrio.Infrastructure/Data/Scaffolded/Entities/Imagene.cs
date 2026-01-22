using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PedidosBarrio.Infrastructure.Data.Scaffolded.Entities;

public partial class Imagene
{
    [Key]
    [Column("ImagenID")]
    public int ImagenId { get; set; }

    public int? ExternalId { get; set; }

    [Column("URLImagen")]
    [StringLength(500)]
    public string? Urlimagen { get; set; }

    public string? Descripcion { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public bool? Activa { get; set; }

    [StringLength(10)]
    public string? Type { get; set; }

    [Column("order")]
    public short Order { get; set; }

    [Column("EmpresaID")]
    public Guid? EmpresaId { get; set; }
}
