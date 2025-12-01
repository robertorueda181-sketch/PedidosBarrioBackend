using MediatR;

namespace PedidosBarrio.Application.Commands.DeleteEmpresa
{
    public class DeleteEmpresaCommand : IRequest<Unit>
    {
        public Guid EmpresaID { get; set; }

        public DeleteEmpresaCommand(Guid empresaID)
        {
            EmpresaID = empresaID;
        }
    }
}
