using MediatR;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.DeleteSuscripcion
{
    public class DeleteSuscripcionCommandHandler : IRequestHandler<DeleteSuscripcionCommand, Unit>
    {
        private readonly ISuscripcionRepository _suscripcionRepository;

        public DeleteSuscripcionCommandHandler(ISuscripcionRepository suscripcionRepository)
        {
            _suscripcionRepository = suscripcionRepository;
        }

        public async Task<Unit> Handle(DeleteSuscripcionCommand command, CancellationToken cancellationToken)
        {
            var suscripcion = await _suscripcionRepository.GetByIdAsync(command.SuscripcionID);
            if (suscripcion == null)
            {
                throw new ApplicationException($"Suscripcion with ID {command.SuscripcionID} not found.");
            }

            await _suscripcionRepository.DeleteAsync(command.SuscripcionID);
            return Unit.Value;
        }
    }
}
