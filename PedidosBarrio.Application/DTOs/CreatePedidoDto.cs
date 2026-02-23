namespace PedidosBarrio.Application.DTOs;

public class CreatePedidoDto
{
    public string Codigo { get; set; } = null!; // Código del negocio/empresa

    // Datos del cliente
    public ClientePedidoDto Cliente { get; set; } = new();

    // Productos del pedido
    public List<ProductoPedidoDto> Productos { get; set; } = new();

    public string? Observaciones { get; set; }
}

public class ClientePedidoDto
{
    public string DNI { get; set; } = null!;
    public string Nombres { get; set; } = null!;
    public string? Telefono { get; set; }
    public string? DireccionTexto { get; set; }
    public decimal? Latitud { get; set; }
    public decimal? Longitud { get; set; }
    public string? Distrito { get; set; }
    public string? Provincia { get; set; }
    public string? Departamento { get; set; }
}

public class ProductoPedidoDto
{
    public int ProductoID { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}
