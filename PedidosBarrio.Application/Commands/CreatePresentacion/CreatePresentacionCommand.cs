using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.CreatePresentacion
{
    public class CreatePresentacionCommand : IRequest<PresentacionDetalleDto>
    {
        public CreatePresentacionCommand(CreatePresentacionDto dto)
        {
            Descripcion = dto.Descripcion;
            ProductoID = dto.ProductoID;
            Opciones = dto.Opciones;
        }

        public string Descripcion { get; set; }
        public int ProductoID { get; set; }
        public List<CreatePresentacionOpcionDto> Opciones { get; set; }
    }
}
