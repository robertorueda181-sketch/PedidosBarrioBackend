using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetUbigeo
{
    public class GetUbigeoQueryHandler : IRequestHandler<GetUbigeoQuery, IEnumerable<UbigeoDto>>
    {
        private readonly IUbigeoRepository _ubigeoRepository;

        public GetUbigeoQueryHandler(IUbigeoRepository ubigeoRepository)
        {
            _ubigeoRepository = ubigeoRepository;
        }

        public async Task<IEnumerable<UbigeoDto>> Handle(GetUbigeoQuery request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(request.ProvinceId))
            {
                var districts = await _ubigeoRepository.GetDistrictsByProvinceAsync(request.ProvinceId);
                return districts.Select(d => new UbigeoDto
                {
                    Id = d.Id,
                    Name = d.Name ?? ""
                });
            }

            if (!string.IsNullOrEmpty(request.DepartmentId))
            {
                var provinces = await _ubigeoRepository.GetProvincesByDepartmentAsync(request.DepartmentId);
                return provinces.Select(p => new UbigeoDto
                {
                    Id = p.Id,
                    Name = p.Name
                });
            }

            var departments = await _ubigeoRepository.GetDepartmentsAsync();
            return departments.Select(d => new UbigeoDto
            {
                Id = d.Id,
                Name = d.Name
            });
        }
    }
}
