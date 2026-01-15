using MediatR;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.DeleteCategoria
{
    public class DeleteCategoriaCommandHandler : IRequestHandler<DeleteCategoriaCommand>
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteCategoriaCommandHandler(
            ICategoriaRepository categoriaRepository,
            ICurrentUserService currentUserService)
        {
            _categoriaRepository = categoriaRepository;
            _currentUserService = currentUserService;
        }

        public async Task Handle(DeleteCategoriaCommand command, CancellationToken cancellationToken)
        {
            // Obtener EmpresaID del token del usuario logueado
            var empresaIdUsuario = _currentUserService.GetEmpresaId();

            // Verificar que la categoría existe
            var categoria = await _categoriaRepository.GetByIdAsync(command.CategoriaId);
            if (categoria == null)
            {
                throw new ApplicationException($"La categoría con ID {command.CategoriaId} no existe.");
            }

            // Validar que la categoría pertenece a la empresa del usuario logueado
            if (categoria.EmpresaID != empresaIdUsuario)
            {
                throw new UnauthorizedAccessException("No tienes permisos para eliminar esta categoría. Solo puedes eliminar categorías de tu empresa.");
            }

            // Soft delete: set Activo to false instead of hard delete
            await _categoriaRepository.SoftDeleteAsync(command.CategoriaId);
        }
    }
}