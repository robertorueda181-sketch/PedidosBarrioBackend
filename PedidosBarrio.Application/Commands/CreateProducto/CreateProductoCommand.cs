using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.CreateProducto
{
    public class CreateProductoCommand : IRequest<ProductoDto>
    {
        public Guid EmpresaID { get; set; }
        public int CategoriaID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public CreateProductoCommand(Guid empresaID, int categoriaID, string nombre, string descripcion)
        {
            EmpresaID = empresaID;
            CategoriaID = categoriaID;
            Nombre = nombre;
            Descripcion = descripcion;
        }
    }
}
