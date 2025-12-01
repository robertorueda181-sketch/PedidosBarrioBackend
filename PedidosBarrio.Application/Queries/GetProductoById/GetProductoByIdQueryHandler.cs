using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetProductoById
{
    public class GetProductoByIdQueryHandler : IRequestHandler<GetProductoByIdQuery, ProductoDto>
    {
        private readonly IProductoRepository _productoRepository;
        private readonly IMapper _mapper;

        public GetProductoByIdQueryHandler(IProductoRepository productoRepository, IMapper mapper)
        {
            _productoRepository = productoRepository;
            _mapper = mapper;
        }

        public async Task<ProductoDto> Handle(GetProductoByIdQuery query, CancellationToken cancellationToken)
        {
            var producto = await _productoRepository.GetByIdAsync(query.ProductoID);
            if (producto == null)
            {
                return null;
            }
            return _mapper.Map<ProductoDto>(producto);
        }
    }
}
