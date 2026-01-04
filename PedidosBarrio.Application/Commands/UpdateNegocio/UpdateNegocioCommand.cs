using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.UpdateNegocio
{
    public class UpdateNegocioCommand : IRequest<NegocioDto>
    {
        public int NegocioID { get; set; }
        public Guid EmpresaID { get; set; }
        public int TiposID { get; set; }
        public string URLNegocio { get; set; }
        public string Descripcion { get; set; }
        public string URLOpcional { get; set; }

        public UpdateNegocioCommand(int negocioID, Guid empresaID, int tiposID, string urlNegocio, string descripcion, string urlOpcional = null)
        {
            NegocioID = negocioID;
            EmpresaID = empresaID;
            TiposID = tiposID;
            URLNegocio = urlNegocio;
            Descripcion = descripcion;
            URLOpcional = urlOpcional;
        }
    }
}
