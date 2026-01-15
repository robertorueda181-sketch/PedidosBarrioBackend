using FluentValidation;
using FluentValidation.Results;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.UpdateCategoria
{
    public class UpdateCategoriaCommandHandler : IRequestHandler<UpdateCategoriaCommand>
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IValidator<UpdateCategoriaDto> _validator;
        private readonly ICurrentUserService _currentUserService;

        public UpdateCategoriaCommandHandler(
            ICategoriaRepository categoriaRepository, 
            IValidator<UpdateCategoriaDto> validator,
            ICurrentUserService currentUserService)
        {
            _categoriaRepository = categoriaRepository;
            _validator = validator;
            _currentUserService = currentUserService;
        }

        public async Task Handle(UpdateCategoriaCommand command, CancellationToken cancellationToken)
        {
            // Obtener EmpresaID del token del usuario logueado
            var empresaIdUsuario = _currentUserService.GetEmpresaId();

            // Verificar que la categoría existe y pertenece al usuario
            var categoriaExistente = await _categoriaRepository.GetByIdAsync(command.CategoriaId);
            if (categoriaExistente == null)
            {
                throw new ApplicationException($"La categoría con ID {command.CategoriaId} no existe.");
            }

            // Validar que la categoría pertenece a la empresa del usuario logueado
            if (categoriaExistente.EmpresaID != empresaIdUsuario)
            {
                throw new UnauthorizedAccessException("No tienes permisos para actualizar esta categoría. Solo puedes actualizar categorías de tu empresa.");
            }

            // Validate input
            var updateDto = new UpdateCategoriaDto
            {
                Descripcion = command.Descripcion,
                Color = command.Color
            };

            ValidationResult validationResult = await _validator.ValidateAsync(updateDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // Actualizar solo los campos permitidos manteniendo el ID y EmpresaID originales
            var categoria = new Domain.Entities.Categoria(
                categoriaExistente.CategoriaID,
                categoriaExistente.EmpresaID, 
                command.Descripcion, 
                command.Color, 
                categoriaExistente.Activo);

            await _categoriaRepository.UpdateAsync(categoria);
        }
    }
}