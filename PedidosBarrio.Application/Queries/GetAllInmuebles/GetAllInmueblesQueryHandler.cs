using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetAllInmuebles
{
    public class GetAllInmueblesQueryHandler : IRequestHandler<GetAllInmueblesQuery, IEnumerable<InmuebleDto>>
    {
        private readonly IInmuebleRepository _inmuebleRepository;
        private readonly IMapper _mapper;

        public GetAllInmueblesQueryHandler(IInmuebleRepository inmuebleRepository, IMapper mapper)
        {
            _inmuebleRepository = inmuebleRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<InmuebleDto>> Handle(GetAllInmueblesQuery query, CancellationToken cancellationToken)
        {
            // Obtener todos los inmuebles
            var inmuebles = await _inmuebleRepository.GetAllAsync();

            // Mapear a InmuebleDto - incluye Tipo, URLImagen, Operacion
            var result = inmuebles.Select(inmueble => new InmuebleDto
            {
                InmuebleID = inmueble.InmuebleID,
                EmpresaID = inmueble.EmpresaID ?? Guid.Empty,
                TiposID = inmueble.TiposId ?? 0,
                Tipo = inmueble.Tipo ?? string.Empty,                                    // ✅ Viene de la FUNCTION
                Precio = inmueble.Precio ?? 0,
                Medidas = inmueble.Medidas ?? string.Empty,
                Ubicacion = inmueble.Ubicacion ?? string.Empty,
                Dormitorios = inmueble.Dormitorios ?? 0,
                Banos = inmueble.Banos ?? 0,
                Descripcion = inmueble.Descripcion ?? string.Empty,
                URLImagen = inmueble.Imagen?.URLImagen ?? string.Empty,                 // ✅ Viene de la FUNCTION
                Operacion = inmueble.Operacion?.Descripcion ?? string.Empty             // ✅ Viene de la FUNCTION
            }).ToList();

            return result;
        }
    }
}


