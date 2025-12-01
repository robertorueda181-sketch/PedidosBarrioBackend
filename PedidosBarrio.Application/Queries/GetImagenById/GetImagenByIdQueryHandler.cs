using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetImagenById
{
    public class GetImagenByIdQueryHandler : IRequestHandler<GetImagenByIdQuery, ImagenDto>
    {
        private readonly IImagenRepository _imagenRepository;
        private readonly IMapper _mapper;

        public GetImagenByIdQueryHandler(IImagenRepository imagenRepository, IMapper mapper)
        {
            _imagenRepository = imagenRepository;
            _mapper = mapper;
        }

        public async Task<ImagenDto> Handle(GetImagenByIdQuery query, CancellationToken cancellationToken)
        {
            var imagen = await _imagenRepository.GetByIdAsync(query.ImagenID);
            if (imagen == null)
            {
                return null;
            }
            return _mapper.Map<ImagenDto>(imagen);
        }
    }
}
