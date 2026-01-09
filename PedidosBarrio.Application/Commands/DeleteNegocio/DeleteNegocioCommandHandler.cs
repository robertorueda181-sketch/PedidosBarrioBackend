using MediatR;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.DeleteNegocio
{
    public class DeleteNegocioCommandHandler : IRequestHandler<DeleteNegocioCommand, Unit>
    {
        private readonly INegocioRepository _negocioRepository;

        public DeleteNegocioCommandHandler(INegocioRepository negocioRepository)
        {
            _negocioRepository = negocioRepository;
        }

        public async Task<Unit> Handle(DeleteNegocioCommand command, CancellationToken cancellationToken)
        {
            var negocio = await _negocioRepository.GetByIdAsync(command.NegocioID.ToString());
            if (negocio == null)
            {
                throw new ApplicationException($"Negocio with ID {command.NegocioID} not found.");
            }

            await _negocioRepository.DeleteAsync(command.NegocioID);
            return Unit.Value;
        }
    }
}
