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
                EmpresaID = inmueble.EmpresaID,
                TiposID = inmueble.TiposID,
                Tipo = inmueble.Tipo,                                    // ✅ Viene de la FUNCTION
                Precio = inmueble.Precio,
                Medidas = inmueble.Medidas,
                Ubicacion = inmueble.Ubicacion,
                Dormitorios = inmueble.Dormitorios,
                Banos = inmueble.Banos,
                Descripcion = inmueble.Descripcion,
                URLImagen = inmueble.Imagen?.URLImagen,                 // ✅ Viene de la FUNCTION
                Operacion = inmueble.Operacion?.Descripcion             // ✅ Viene de la FUNCTION
            }).ToList();

            return result;
        }
    }
}


