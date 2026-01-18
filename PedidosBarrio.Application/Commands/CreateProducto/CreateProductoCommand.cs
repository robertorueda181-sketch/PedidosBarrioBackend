using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.CreateProducto
{
    public class CreateProductoCommand : IRequest<ProductoDto>
    {
        public int CategoriaID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string Imagen { get; set; }

        public CreateProductoCommand(CreateProductoDto dto)
        {
            CategoriaID = dto.CategoriaID;
            Nombre = dto.Nombre;
            Descripcion = dto.Descripcion;
            Precio = dto.Precio;
            Stock = dto.Stock;
            Imagen = dto.Imagen;
        }
    }
}
