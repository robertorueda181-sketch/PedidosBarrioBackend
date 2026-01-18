using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.UpdateProducto
{
    public class UpdateProductoCommand : IRequest<ProductoDto>
    {
        public int ProductoID { get; set; }
        public int CategoriaID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string Imagen { get; set; }

        public UpdateProductoCommand(int productoId, UpdateProductoDto dto)
        {
            ProductoID = productoId;
            CategoriaID = dto.CategoriaID;
            Nombre = dto.Nombre;
            Descripcion = dto.Descripcion;
            Precio = dto.Precio;
            Stock = dto.Stock;
            Imagen = dto.Imagen;
        }
    }
}
