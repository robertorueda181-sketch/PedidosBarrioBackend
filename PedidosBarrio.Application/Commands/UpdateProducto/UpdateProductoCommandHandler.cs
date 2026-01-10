using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.UpdateProducto
{
    public class UpdateProductoCommandHandler : IRequestHandler<UpdateProductoCommand, ProductoDto>
    {
        private readonly IProductoRepository _productoRepository;
        private readonly IMapper _mapper;

        public UpdateProductoCommandHandler(IProductoRepository productoRepository, IMapper mapper)
        {
            _productoRepository = productoRepository;
            _mapper = mapper;
        }

        public async Task<ProductoDto> Handle(UpdateProductoCommand command, CancellationToken cancellationToken)
        {
            var producto = await _productoRepository.GetByIdAsync(command.ProductoID);
            if (producto == null)
            {
                throw new ApplicationException($"Producto with ID {command.ProductoID} not found.");
            }

            producto = new Producto(command.EmpresaID, command.Nombre, command.Descripcion)
            {
                ProductoID = command.ProductoID,
                FechaCreacion = producto.FechaCreacion,
                CategoriaID = command.CategoriaID
            };

            await _productoRepository.UpdateAsync(producto);
            return _mapper.Map<ProductoDto>(producto);
        }
    }
}
