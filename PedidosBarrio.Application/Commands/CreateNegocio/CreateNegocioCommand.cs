using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.CreateNegocio
{
    public class CreateNegocioCommand : IRequest<NegocioDto>
    {
        public Guid EmpresaID { get; set; }
        public int TiposID { get; set; }
        public string URLNegocio { get; set; }
        public string Descripcion { get; set; }
        public string URLOpcional { get; set; }

        public CreateNegocioCommand(Guid empresaID, int tiposID, string urlNegocio, string descripcion, string urlOpcional = null)
        {
            EmpresaID = empresaID;
            TiposID = tiposID;
            URLNegocio = urlNegocio;
            Descripcion = descripcion;
            URLOpcional = urlOpcional;
        }
    }
}
