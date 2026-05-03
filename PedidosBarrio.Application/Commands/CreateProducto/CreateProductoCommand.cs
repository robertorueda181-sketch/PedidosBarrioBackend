using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.CreateProducto
{
    public class CreateProductoCommand : IRequest<ProductoDto>
    {
        public string Codigo { get; }
        public string CategoriaDescripcion { get; }
        public string Nombre { get; }
        public string Descripcion { get; }
        public int Stock { get; }
        public int? StockMinimo { get; }
        public bool Inventario { get; }

        // Lista de precios
        public List<PrecioCreateDto> Precios { get; } = new List<PrecioCreateDto>();

        // Imagen
        public string ImagenUrl { get; }
        public string ImagenDescripcion { get; }

        public CreateProductoCommand(CreateProductoDto dto)
        {
            Codigo = dto.Codigo;
            CategoriaDescripcion = dto.CategoriaDescripcion;
            Nombre = dto.Nombre;
            Descripcion = dto.Descripcion;
            Stock = dto.Stock;
            StockMinimo = dto.StockMinimo;
            Inventario = dto.Inventario;
            Precios = dto.Precios;
            ImagenUrl = dto.ImagenUrl;
            ImagenDescripcion = dto.ImagenDescripcion;
        }
    }
}
