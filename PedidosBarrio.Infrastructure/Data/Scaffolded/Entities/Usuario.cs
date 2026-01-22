using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PedidosBarrio.Infrastructure.Data.Scaffolded.Entities;

[Index("Email", Name = "Usuarios_Email_key", IsUnique = true)]
[Index("Email", Name = "idx_usuarios_email")]
public partial class Usuario
{
    [Key]
    [Column("UsuarioID")]
    public Guid UsuarioId { get; set; }

    [StringLength(100)]
    public string NombreUsuario { get; set; } = null!;

    [StringLength(255)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    public string ContrasenaHash { get; set; } = null!;

    [StringLength(255)]
    public string ContrasenaSalt { get; set; } = null!;

    public DateTime? FechaRegistro { get; set; }

    public bool? Activa { get; set; }

    [StringLength(50)]
    public string? SocialId { get; set; }

    [StringLength(8)]
    public string? Provider { get; set; }
}
