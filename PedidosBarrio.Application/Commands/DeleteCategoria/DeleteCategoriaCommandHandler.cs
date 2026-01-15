using MediatR;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.DeleteCategoria
{
    public class DeleteCategoriaCommandHandler : IRequestHandler<DeleteCategoriaCommand>
    {
        private readonly ICategoriaRepository _categoriaRepository;

        public DeleteCategoriaCommandHandler(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        public async Task Handle(DeleteCategoriaCommand command, CancellationToken cancellationToken)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(command.CategoriaId);
            
            if (categoria == null)
            {
                throw new ApplicationException($"Categoria with ID {command.CategoriaId} not found.");
            }

            // Soft delete: set Activo to false instead of hard delete
            await _categoriaRepository.SoftDeleteAsync(command.CategoriaId);
        }
    }
}