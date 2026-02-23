using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedidosBarrio.Domain.Entities;

public partial class PedidoDetalle
{
    public PedidoDetalle() { }

    public PedidoDetalle(long pedidoId, int productoId, int cantidad, decimal precioUnitario)
    {
        PedidoID = pedidoId;
        ProductoID = productoId;
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
        Subtotal = cantidad * precioUnitario;
    }

    [Key]
    [Column("PedidoDetalleID")]
    public long PedidoDetalleID { get; set; }

    [Column("PedidoID")]
    public long PedidoID { get; set; }

    [Column("ProductoID")]
    public int ProductoID { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }

    [ForeignKey("PedidoID")]
    [InverseProperty("PedidoDetalles")]
    public virtual Pedido? Pedido { get; set; }

    [ForeignKey("ProductoID")]
    [InverseProperty("PedidoDetalles")]
    public virtual Producto? Producto { get; set; }
}
