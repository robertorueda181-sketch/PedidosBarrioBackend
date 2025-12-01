using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.CreateImagen
{
    public class CreateImagenCommand : IRequest<ImagenDto>
    {
        public int ProductoID { get; set; }
        public string URLImagen { get; set; }
        public string Descripcion { get; set; }

        public CreateImagenCommand(int productoID, string urlImagen, string descripcion = null)
        {
            ProductoID = productoID;
            URLImagen = urlImagen;
            Descripcion = descripcion;
        }
    }
}
