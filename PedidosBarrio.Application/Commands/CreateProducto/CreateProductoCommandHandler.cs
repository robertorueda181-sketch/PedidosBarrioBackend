using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.CreateProducto
{
    public class CreateProductoCommandHandler : IRequestHandler<CreateProductoCommand, ProductoDto>
    {
        private readonly IProductoRepository _productoRepository;
        private readonly IMapper _mapper;

        public CreateProductoCommandHandler(IProductoRepository productoRepository, IMapper mapper)
        {
            _productoRepository = productoRepository;
            _mapper = mapper;
        }

        public async Task<ProductoDto> Handle(CreateProductoCommand command, CancellationToken cancellationToken)
        {
            var producto = new Producto(command.EmpresaID, command.Nombre, command.Descripcion);

            await _productoRepository.AddAsync(producto);
            return _mapper.Map<ProductoDto>(producto);
        }
    }
}
