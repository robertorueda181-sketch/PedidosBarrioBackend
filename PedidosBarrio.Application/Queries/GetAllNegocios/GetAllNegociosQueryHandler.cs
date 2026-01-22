using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetAllNegocios
{
    public class GetAllNegociosQueryHandler : IRequestHandler<GetAllNegociosQuery, IEnumerable<NegocioDto>>
    {
        private readonly INegocioRepository _negocioRepository;
        private readonly IImagenRepository _imagenRepository;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly IMapper _mapper;

        public GetAllNegociosQueryHandler(
            INegocioRepository negocioRepository, 
            IImagenRepository imagenRepository,
            IImageProcessingService imageProcessingService,
            IMapper mapper)
        {
            _negocioRepository = negocioRepository;
            _imagenRepository = imagenRepository;
            _imageProcessingService = imageProcessingService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<NegocioDto>> Handle(GetAllNegociosQuery query, CancellationToken cancellationToken)
        {
            var negocios = await _negocioRepository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<NegocioDto>>(negocios).ToList();

            // Para cada negocio, buscar su imagen asociada
            foreach (var dto in dtos)
            {
                var imagenes = await _imagenRepository.GetByProductoIdAsync(dto.NegocioID, "NEG");
                var principal = imagenes.FirstOrDefault();
                if (principal != null && !string.IsNullOrEmpty(principal.URLImagen))
                {
                    dto.UrlImagen = await _imageProcessingService.GetImageUrlAsync(principal.URLImagen);
                }
            }

            return dtos;
        }
    }
}
