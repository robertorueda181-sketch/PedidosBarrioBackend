using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.CreateProducto
{
    public class CreateProductoCommand : IRequest<ProductoDto>
    {
        public int EmpresaID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public CreateProductoCommand(int empresaID, string nombre, string descripcion)
        {
            EmpresaID = empresaID;
            Nombre = nombre;
            Descripcion = descripcion;
        }
    }
}
