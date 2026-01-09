using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetNegocioByCodigoEmpresa
{
    public class GetNegocioByCodigoEmpresaQuery : IRequest<NegocioDetalleDto>
    {
        public string CodigoEmpresa { get; set; }

        public GetNegocioByCodigoEmpresaQuery(string codigoEmpresa)
        {
            CodigoEmpresa = codigoEmpresa;
        }
    }
}
