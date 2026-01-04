using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetAllInmuebles
{
    /// <summary>
    /// Handler para obtener TODOS los inmuebles CON detalles (Tipos e Imagenes)
    /// Ejecuta la FUNCTION: sp_GetAllInmuebles() que incluye JOINs
    /// </summary>
    public class GetAllInmueblesDetailsQueryHandler : IRequestHandler<GetAllInmueblesDetailsQuery, IEnumerable<InmuebleDetailsDto>>
    {
        private readonly IInmuebleRepository _inmuebleRepository;
        private readonly IMapper _mapper;

        public GetAllInmueblesDetailsQueryHandler(IInmuebleRepository inmuebleRepository, IMapper mapper)
        {
            _inmuebleRepository = inmuebleRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<InmuebleDetailsDto>> Handle(GetAllInmueblesDetailsQuery query, CancellationToken cancellationToken)
        {
            // Obtener inmuebles con todos sus datos (la función SQL hará los JOINs)
            var inmuebles = await _inmuebleRepository.GetAllAsync();
            
            // Mapear a DTO con detalles
            // Nota: Esta es una solución temporal
            // Lo ideal sería tener un método específico en el repository para esta query
            return inmuebles.Select(i => new InmuebleDetailsDto
            {
                InmuebleID = i.InmuebleID,
                EmpresaID = i.EmpresaID,
                TiposID = i.TiposID,
                OperacionID = i.OperacionID,
                Precio = i.Precio,
                Medidas = i.Medidas,
                Ubicacion = i.Ubicacion,
                Dormitorios = i.Dormitorios,
                Banos = i.Banos,
                Descripcion = i.Descripcion
            }).ToList();
        }
    }
}

