using MediatR;

namespace PedidosBarrio.Application.Commands.DeleteImagen
{
    public class DeleteImagenCommand : IRequest<Unit>
    {
        public int ImagenID { get; set; }

        public DeleteImagenCommand(int imagenID)
        {
            ImagenID = imagenID;
        }
    }
}
