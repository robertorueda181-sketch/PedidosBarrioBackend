using MediatR;

namespace PedidosBarrio.Application.Commands.DeleteEmpresa
{
    public class DeleteCompanyCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }

        public DeleteCompanyCommand(Guid id)
        {
            Id = id;
        }
    }
}
