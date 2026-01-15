using FluentValidation;
using FluentValidation.Results;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.UpdateCategoria
{
    public class UpdateCategoriaCommandHandler : IRequestHandler<UpdateCategoriaCommand>
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IValidator<UpdateCategoriaDto> _validator;

        public UpdateCategoriaCommandHandler(ICategoriaRepository categoriaRepository, IValidator<UpdateCategoriaDto> validator)
        {
            _categoriaRepository = categoriaRepository;
            _validator = validator;
        }

        public async Task Handle(UpdateCategoriaCommand command, CancellationToken cancellationToken)
        {
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

            var categoria = new Domain.Entities.Categoria(command.Descripcion,command.Color);


            await _categoriaRepository.UpdateAsync(categoria);
        }
    }
}