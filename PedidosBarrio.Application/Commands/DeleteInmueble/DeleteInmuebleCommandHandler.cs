using MediatR;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.DeleteInmueble
{
    public class DeleteInmuebleCommandHandler : IRequestHandler<DeleteInmuebleCommand, Unit>
    {
        private readonly IInmuebleRepository _inmuebleRepository;

        public DeleteInmuebleCommandHandler(IInmuebleRepository inmuebleRepository)
        {
            _inmuebleRepository = inmuebleRepository;
        }

        public async Task<Unit> Handle(DeleteInmuebleCommand command, CancellationToken cancellationToken)
        {
            var inmueble = await _inmuebleRepository.GetByIdAsync(command.InmuebleID);
            if (inmueble == null)
            {
                throw new ApplicationException($"Inmueble with ID {command.InmuebleID} not found.");
            }

            await _inmuebleRepository.DeleteAsync(command.InmuebleID);
            return Unit.Value;
        }
    }
}
