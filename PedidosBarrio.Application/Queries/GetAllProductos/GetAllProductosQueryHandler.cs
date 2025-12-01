using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetAllProductos
{
    public class GetAllProductosQueryHandler : IRequestHandler<GetAllProductosQuery, IEnumerable<ProductoDto>>
    {
        private readonly IProductoRepository _productoRepository;
        private readonly IMapper _mapper;

        public GetAllProductosQueryHandler(IProductoRepository productoRepository, IMapper mapper)
        {
            _productoRepository = productoRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductoDto>> Handle(GetAllProductosQuery query, CancellationToken cancellationToken)
        {
            var productos = await _productoRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductoDto>>(productos);
        }
    }
}
