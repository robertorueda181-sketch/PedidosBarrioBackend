using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetAllImagenes
{
    public class GetAllImagenesQueryHandler : IRequestHandler<GetAllImagenesQuery, IEnumerable<ImagenDto>>
    {
        private readonly IImagenRepository _imagenRepository;
        private readonly IMapper _mapper;

        public GetAllImagenesQueryHandler(IImagenRepository imagenRepository, IMapper mapper)
        {
            _imagenRepository = imagenRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ImagenDto>> Handle(GetAllImagenesQuery query, CancellationToken cancellationToken)
        {
            var imagenes = await _imagenRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ImagenDto>>(imagenes);
        }
    }
}
