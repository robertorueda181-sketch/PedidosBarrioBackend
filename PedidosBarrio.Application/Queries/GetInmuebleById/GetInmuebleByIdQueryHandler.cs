using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetInmuebleById
{
    public class GetInmuebleByIdQueryHandler : IRequestHandler<GetInmuebleByIdQuery, InmuebleDetailsDto>
    {
        private readonly IInmuebleRepository _inmuebleRepository;
        private readonly IImagenRepository _imagenRepository;
        private readonly IMapper _mapper;

        public GetInmuebleByIdQueryHandler(IInmuebleRepository inmuebleRepository, IImagenRepository imagenRepository, IMapper mapper)
        {
            _inmuebleRepository = inmuebleRepository;
            _imagenRepository = imagenRepository;
            _mapper = mapper;
        }

        public async Task<InmuebleDetailsDto> Handle(GetInmuebleByIdQuery query, CancellationToken cancellationToken)
        {
            var inmueble = await _inmuebleRepository.GetByIdAsync(query.InmuebleID);
            if (inmueble == null)
            {
                return null;
            }

            // Construir el DTO manualmente porque Inmueble ya tiene los datos de Tipo, URLImagen y Operacion
            var result = new InmuebleDetailsDto
            {
                InmuebleID = inmueble.InmuebleID,
                EmpresaID = inmueble.EmpresaID ?? Guid.Empty,
                TiposID = inmueble.TiposId ?? 0,
                TipoInmueble = inmueble.Tipo ?? string.Empty,                         
                OperacionID = inmueble.OperacionID ?? 0,
                TipoOperacion = inmueble.Operacion?.Descripcion ?? string.Empty,       
                Precio = inmueble.Precio ?? 0,
                Medidas = inmueble.Medidas ?? string.Empty,
                Ubicacion = inmueble.Ubicacion ?? string.Empty,
                Dormitorios = inmueble.Dormitorios ?? 0,
                Banos = inmueble.Banos ?? 0,
                Descripcion = inmueble.Descripcion ?? string.Empty,
                Latitud = inmueble.Latitud?.ToString() ?? string.Empty,
                Longitud = inmueble.Longitud?.ToString() ?? string.Empty,
                Imagenes = _mapper.Map<List<ImagenUrlDto>>(
                    await _imagenRepository.GetByProductoIdAsync(query.InmuebleID, "inm"))
            };

            return result;
        }
    }
}


