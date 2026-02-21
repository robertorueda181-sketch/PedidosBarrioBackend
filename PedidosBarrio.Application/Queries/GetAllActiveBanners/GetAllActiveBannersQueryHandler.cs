using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetAllActiveBanners
{
    public class GetAllActiveBannersQueryHandler : IRequestHandler<GetAllActiveBannersQuery, IEnumerable<PublicBannerDto>>
    {
        private readonly IBannerRepository _bannerRepository;

        public GetAllActiveBannersQueryHandler(IBannerRepository bannerRepository)
        {
            _bannerRepository = bannerRepository;
        }

        public async Task<IEnumerable<PublicBannerDto>> Handle(GetAllActiveBannersQuery request, CancellationToken cancellationToken)
        {
            var banners = await _bannerRepository.GetAllActiveAsync();

            return banners.Select(b => new PublicBannerDto
            {
                Titulo = b.Titulo,
                Descripcion = b.Descripcion,
                Link = b.Link,
                UrlImagen = b.UrlImagen,
                TextoBoton = b.TextoBoton
            }).ToList();
        }
    }
}
