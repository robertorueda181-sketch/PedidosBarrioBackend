using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedidosBarrio.Domain.Entities;

public partial class Cliente
{
    public Cliente() { }

    public Cliente(Guid usuarioId, string dni, string nombres)
    {
        ClienteID = Guid.NewGuid();
        UsuarioID = usuarioId;
        DNI = dni;
        Nombres = nombres;
        FechaRegistro = DateTime.UtcNow;
        Activo = true;
    }

    [Key]
    [Column("ClienteID")]
    public Guid ClienteID { get; set; }

    [Column("UsuarioID")]
    public Guid UsuarioID { get; set; }

    [StringLength(20)]
    public string DNI { get; set; } = null!;

    [StringLength(100)]
    public string Nombres { get; set; } = null!;

    [StringLength(255)]
    public string? ContrasenaHash { get; set; }

    [StringLength(255)]
    public string? ContrasenaSalt { get; set; }

    [StringLength(20)]
    public string? Telefono { get; set; }

    [StringLength(500)]
    public string? DireccionTexto { get; set; }

    public decimal? Latitud { get; set; }

    public decimal? Longitud { get; set; }

    [StringLength(100)]
    public string? Distrito { get; set; }

    [StringLength(100)]
    public string? Provincia { get; set; }

    [StringLength(100)]
    public string? Departamento { get; set; }

    // OAuth fields
    [StringLength(50)]
    public string? Provider { get; set; } // "google", "facebook", etc.

    [StringLength(255)]
    public string? ProviderUserId { get; set; } // External user ID from provider

    [StringLength(255)]
    public string? Email { get; set; } // From OAuth or manual entry

    public DateTime FechaRegistro { get; set; }

    public bool Activo { get; set; }

        [ForeignKey("UsuarioID")]
        [InverseProperty("Clientes")]
        public virtual Usuario? Usuario { get; set; }

        [InverseProperty("Cliente")]
        public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

        [InverseProperty("Cliente")]
        public virtual ICollection<ClienteDireccion> Direcciones { get; set; } = new List<ClienteDireccion>();
    }
