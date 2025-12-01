using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetImagenById
{
    public class GetImagenByIdQuery : IRequest<ImagenDto>
    {
        public int ImagenID { get; set; }

        public GetImagenByIdQuery(int imagenID)
        {
            ImagenID = imagenID;
        }
    }
}
