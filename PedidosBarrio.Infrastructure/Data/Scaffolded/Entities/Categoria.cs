using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PedidosBarrio.Infrastructure.Data.Scaffolded.Entities;

public partial class Categoria
{
    [Key]
    [Column("CategoriaID")]
    public short CategoriaId { get; set; }

    [Column("EmpresaID")]
    public Guid EmpresaId { get; set; }

    [StringLength(50)]
    public string Descripcion { get; set; } = null!;

    public bool? Activo { get; set; }

    [StringLength(15)]
    public string? Color { get; set; }
}
