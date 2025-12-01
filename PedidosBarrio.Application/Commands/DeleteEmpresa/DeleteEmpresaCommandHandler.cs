using MediatR;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.DeleteEmpresa
{
    public class DeleteEmpresaCommandHandler : IRequestHandler<DeleteEmpresaCommand, Unit>
    {
        private readonly IEmpresaRepository _empresaRepository;

        public DeleteEmpresaCommandHandler(IEmpresaRepository empresaRepository)
        {
            _empresaRepository = empresaRepository;
        }

        public async Task<Unit> Handle(DeleteEmpresaCommand command, CancellationToken cancellationToken)
        {
            var empresa = await _empresaRepository.GetByIdAsync(command.EmpresaID);
            if (empresa == null)
            {
                throw new ApplicationException($"Empresa with ID {command.EmpresaID} not found.");
            }

            await _empresaRepository.DeleteAsync(command.EmpresaID);
            return Unit.Value;
        }
    }
}
