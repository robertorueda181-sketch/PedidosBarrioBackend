using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.CreateProducto
{
    public class CreateProductoCommand : IRequest<ProductoDto>
    {
        public short CategoriaID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Stock { get; set; }
        public int? StockMinimo { get; set; }
        public bool Inventario { get; set; }

        // Lista de precios
        public List<PrecioCreateDto> Precios { get; set; } = new List<PrecioCreateDto>();

        // Imagen
        public string ImagenUrl { get; set; }
        public string ImagenDescripcion { get; set; }

        public CreateProductoCommand(CreateProductoDto dto)
        {
            CategoriaID = dto.CategoriaID;
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
