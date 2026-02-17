using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PedidosBarrio.Domain.Entities;

public partial class Empresa
{
    public Empresa() { }

    public Empresa(Guid usuarioId, short? tipoEmpresa)
    {
        ID = Guid.NewGuid();
        UsuarioID = usuarioId;
        TipoEmpresa = tipoEmpresa;
        FechaRegistro = DateTime.UtcNow;
        Activa = true;
        Visible = true;
        Aprobado = false;
    }

    [Key]
    [Column("EmpresaID")]
    public Guid ID { get; set; }

    [NotMapped]
    public Guid EmpresaID { get => ID; set => ID = value; }

    [NotMapped]
    public string? Referencia { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public bool? Activa { get; set; }

    [Column("UsuarioID")]
    public Guid? UsuarioID { get; set; }

    public short? TipoEmpresa { get; set; }

    public bool Visible { get; set; }

    public bool Aprobado { get; set; }

    public string? Facebook { get; set; }

    [Column("Instragram")]
    public string? Instagram { get; set; }

    public string? Twitter { get; set; }

    public string? Tiktok { get; set; }

    public string? Whatsapp { get; set; }

    [Column("TelefonoPrincipal")]
    public string? TelefonoPrincipal { get; set; }

    [Column("TelefonoSec")]
    public string? TelefonoSec { get; set; }

    [ForeignKey("UsuarioID")]
    [InverseProperty("Empresas")]
    public virtual Usuario? Usuario { get; set; }

    [InverseProperty("Empresa")]
    public virtual ICollection<Inmueble> Inmuebles { get; set; } = new List<Inmueble>();

    [InverseProperty("Empresa")]
    public virtual ICollection<Negocio> Negocios { get; set; } = new List<Negocio>();

    [InverseProperty("Empresa")]
    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();

    [InverseProperty("Empresa")]
    public virtual ICollection<Suscripcion> Suscripcions { get; set; } = new List<Suscripcion>();

    [InverseProperty("Empresa")]
    public virtual ICollection<Direccion> Direcciones { get; set; } = new List<Direccion>();
}







