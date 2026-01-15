using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.UpdateProducto
{
    public class UpdateProductoCommand : IRequest<ProductoDto>
    {
        public int ProductoID { get; set; }
        public Guid EmpresaID { get; set; }
        public int CategoriaID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public UpdateProductoCommand(int productoID, Guid empresaID, int categoriaID, string nombre, string descripcion)
        {
            ProductoID = productoID;
            EmpresaID = empresaID;
            CategoriaID = categoriaID;
            Nombre = nombre;
            Descripcion = descripcion;
        }
    }
}
