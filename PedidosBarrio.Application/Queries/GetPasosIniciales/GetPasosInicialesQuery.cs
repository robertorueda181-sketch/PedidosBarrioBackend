using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetPasosIniciales;

public class GetPasosInicialesQuery : IRequest<IEnumerable<PasoInicialDto>>
{
    public Guid EmpresaId { get; set; }

    public GetPasosInicialesQuery(Guid empresaId)
    {
        EmpresaId = empresaId;
    }
}
