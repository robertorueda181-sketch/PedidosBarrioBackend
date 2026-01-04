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
                EmpresaID = inmueble.EmpresaID,
                TiposID = inmueble.TiposID,
                TipoInmueble = inmueble.Tipo,                         
                OperacionID = inmueble.OperacionID,
                TipoOperacion = inmueble.Operacion?.Descripcion,       
                Precio = inmueble.Precio,
                Medidas = inmueble.Medidas,
                Ubicacion = inmueble.Ubicacion,
                Dormitorios = inmueble.Dormitorios,
                Banos = inmueble.Banos,
                Descripcion = inmueble.Descripcion,
                Latitud = inmueble.Latitud,
                Longitud = inmueble.Longitud,
                Imagenes = _mapper.Map<List<ImagenUrlDto>>(
                    await _imagenRepository.GetByProductoIdAsync(query.InmuebleID, "inm"))
            };

            return result;
        }
    }
}

