using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PedidosBarrio.Domain.Entities;

[Index("Email", Name = "Usuarios_Email_key", IsUnique = true)]
[Index("Email", Name = "idx_usuarios_email")]
public partial class Usuario
{
    public Usuario() { }

    public Usuario(string nombreUsuario, string email, string contrasenaHash, string contrasenaSalt)
    {
        ID = Guid.NewGuid();
        NombreUsuario = nombreUsuario;
        Email = email;
        ContrasenaHash = contrasenaHash;
            ContrasenaSalt = contrasenaSalt;
            FechaRegistro = DateTime.UtcNow;
            Activa = true;
        }

    [Key]
    [Column("UsuarioID")]
    public Guid ID { get; set; }

    [NotMapped]
    public Guid UsuarioId { get => ID; set => ID = value; }

    [NotMapped]
    public Guid EmpresaID { get; set; }

    [NotMapped]
    public bool Activo { get => Activa ?? false; set => Activa = value; }

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

        [InverseProperty("Usuario")]
        public virtual ICollection<Empresa> Empresas { get; set; } = new List<Empresa>();
    }



