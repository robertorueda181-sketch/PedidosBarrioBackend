using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetUbigeo
{
    public class GetUbigeoQuery : IRequest<IEnumerable<UbigeoDto>>
    {
        public string? DepartmentId { get; set; }
        public string? ProvinceId { get; set; }
    }
}
