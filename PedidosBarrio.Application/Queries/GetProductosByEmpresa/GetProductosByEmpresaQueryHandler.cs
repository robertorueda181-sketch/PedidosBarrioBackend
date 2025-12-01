using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetProductosByEmpresa
{
    public class GetProductosByEmpresaQueryHandler : IRequestHandler<GetProductosByEmpresaQuery, IEnumerable<ProductoDto>>
    {
        private readonly IProductoRepository _productoRepository;
        private readonly IMapper _mapper;

        public GetProductosByEmpresaQueryHandler(IProductoRepository productoRepository, IMapper mapper)
        {
            _productoRepository = productoRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductoDto>> Handle(GetProductosByEmpresaQuery query, CancellationToken cancellationToken)
        {
            var productos = await _productoRepository.GetByEmpresaIdAsync(query.EmpresaID);
            return _mapper.Map<IEnumerable<ProductoDto>>(productos);
        }
    }
}
