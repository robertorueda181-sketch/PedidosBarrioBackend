using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetImagenesByProducto
{
    public class GetImagenesByProductoQueryHandler : IRequestHandler<GetImagenesByProductoQuery, IEnumerable<ImagenDto>>
    {
        private readonly IImagenRepository _imagenRepository;
        private readonly IMapper _mapper;

        public GetImagenesByProductoQueryHandler(IImagenRepository imagenRepository, IMapper mapper)
        {
            _imagenRepository = imagenRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ImagenDto>> Handle(GetImagenesByProductoQuery query, CancellationToken cancellationToken)
        {
            var imagenes = await _imagenRepository.GetByProductoIdAsync(query.ProductoID);
            return _mapper.Map<IEnumerable<ImagenDto>>(imagenes);
        }
    }
}
