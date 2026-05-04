using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetProductoById
{
    public class GetProductoByIdQueryHandler : IRequestHandler<GetProductoByIdQuery, ProductoDto>
    {
        private readonly IProductoRepository _productoRepository;
        private readonly IPresentacionRepository _presentacionRepository;
        private readonly IImagenRepository _imagenRepository;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetProductoByIdQueryHandler(
            IProductoRepository productoRepository,
            IPresentacionRepository presentacionRepository,
            IImagenRepository imagenRepository,
            IImageProcessingService imageProcessingService,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _productoRepository = productoRepository;
            _imagenRepository = imagenRepository;
            _imageProcessingService = imageProcessingService;
            _currentUserService = currentUserService;
            _presentacionRepository = presentacionRepository;
            _mapper = mapper;
        }

        public async Task<ProductoDto> Handle(GetProductoByIdQuery query, CancellationToken cancellationToken)
        {
            // If the user is not logged in, this will fail by design
            var empresaId = _currentUserService.GetEmpresaId();

            var producto = await _productoRepository.GetByIdAsync(query.ProductoID, empresaId);
            if (producto == null)
            {
                return null;
            }

            var dto = _mapper.Map<ProductoDto>(producto);

            var precioPrincipal = producto.Presentaciones
                    .SelectMany(p => p.Opciones)
                    .Where(o => o.Activa && o.EsPrincipal)
                    .Select(o => o.Precio)
                    .FirstOrDefault();

            dto.PrecioActual = precioPrincipal;
           

            return dto;
        }
    }
}
