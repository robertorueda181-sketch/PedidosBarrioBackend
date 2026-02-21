using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetAllActiveBanners
{
    public class GetAllActiveBannersQuery : IRequest<IEnumerable<PublicBannerDto>>
    {
    }
}
