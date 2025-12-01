using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.UpdateProducto
{
    public class UpdateProductoCommand : IRequest<ProductoDto>
    {
        public int ProductoID { get; set; }
        public int EmpresaID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public UpdateProductoCommand(int productoID, int empresaID, string nombre, string descripcion)
        {
            ProductoID = productoID;
            EmpresaID = empresaID;
            Nombre = nombre;
            Descripcion = descripcion;
        }
    }
}
