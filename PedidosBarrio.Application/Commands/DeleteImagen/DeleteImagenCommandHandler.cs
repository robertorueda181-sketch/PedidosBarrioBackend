using MediatR;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.DeleteImagen
{
    public class DeleteImagenCommandHandler : IRequestHandler<DeleteImagenCommand, Unit>
    {
        private readonly IImagenRepository _imagenRepository;

        public DeleteImagenCommandHandler(IImagenRepository imagenRepository)
        {
            _imagenRepository = imagenRepository;
        }

        public async Task<Unit> Handle(DeleteImagenCommand command, CancellationToken cancellationToken)
        {
            var imagen = await _imagenRepository.GetByIdAsync(command.ImagenID);
            if (imagen == null)
            {
                throw new ApplicationException($"Imagen with ID {command.ImagenID} not found.");
            }

            await _imagenRepository.DeleteAsync(command.ImagenID);
            return Unit.Value;
        }
    }
}
