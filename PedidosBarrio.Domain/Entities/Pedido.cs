using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedidosBarrio.Domain.Entities;

public partial class Pedido
{
    public Pedido() { }

    public Pedido(Guid empresaId, Guid clienteId)
    {
        PedidoUID = Guid.NewGuid();
        EmpresaID = empresaId;
        ClienteID = clienteId;
        Estado = "PENDIENTE";
        FechaRegistro = DateTime.UtcNow;
    }

    [Key]
    [Column("PedidoID")]
    public long PedidoID { get; set; }

    [Column("PedidoUID")]
    public Guid PedidoUID { get; set; }

    [Column("EmpresaID")]
    public Guid EmpresaID { get; set; }

    [Column("ClienteID")]
    public Guid ClienteID { get; set; }

    [StringLength(50)]
    public string Estado { get; set; } = "PENDIENTE";

    public decimal? Total { get; set; }

    public string? Observaciones { get; set; }

    public DateTime FechaRegistro { get; set; }

    public DateTime? FechaEntrega { get; set; }

    [ForeignKey("EmpresaID")]
    [InverseProperty("Pedidos")]
    public virtual Empresa? Empresa { get; set; }

    [ForeignKey("ClienteID")]
    [InverseProperty("Pedidos")]
    public virtual Cliente? Cliente { get; set; }

    [InverseProperty("Pedido")]
    public virtual ICollection<PedidoDetalle> PedidoDetalles { get; set; } = new List<PedidoDetalle>();
}
