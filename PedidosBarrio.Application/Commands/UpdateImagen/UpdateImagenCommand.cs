using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.UpdateImagen
{
    public class UpdateImagenCommand : IRequest<ImagenDto>
    {
        public int ImagenID { get; set; }
        public int ProductoID { get; set; }
        public string URLImagen { get; set; }
        public string Descripcion { get; set; }

        public UpdateImagenCommand(int imagenID, int productoID, string urlImagen, string descripcion)
        {
            ImagenID = imagenID;
            ProductoID = productoID;
            URLImagen = urlImagen;
            Descripcion = descripcion;
        }
    }
}
