using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetNegocioByCodigoEmpresa
{
    public class GetNegocioByCodigoEmpresaQueryHandler : IRequestHandler<GetNegocioByCodigoEmpresaQuery, NegocioDetalleDto>
    {
        private readonly IEmpresaRepository _empresaRepository;
        private readonly INegocioRepository _negocioRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IImagenRepository _imagenRepository;
        private readonly IDireccionRepository _direccionRepository;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly IMapper _mapper;

        public GetNegocioByCodigoEmpresaQueryHandler(
            IEmpresaRepository empresaRepository,
            INegocioRepository negocioRepository,
            IProductoRepository productoRepository,
            ICategoriaRepository categoriaRepository,
            IImagenRepository imagenRepository,
            IDireccionRepository direccionRepository,
            IImageProcessingService imageProcessingService,
            IMapper mapper)
        {
            _empresaRepository = empresaRepository;
            _negocioRepository = negocioRepository;
            _productoRepository = productoRepository;
            _categoriaRepository = categoriaRepository;
            _imagenRepository = imagenRepository;
            _direccionRepository = direccionRepository;
            _imageProcessingService = imageProcessingService;
            _mapper = mapper;
        }

        public async Task<NegocioDetalleDto> Handle(GetNegocioByCodigoEmpresaQuery query, CancellationToken cancellationToken)
        {
            var negocio = await _negocioRepository.GetByIdAsync(query.CodigoEmpresa);

            if (negocio == null || negocio.Empresa == null)
                return null;

            var empresa = negocio.Empresa;


            // Obtener imágenes de la empresa para los productos
            var todasLasImagenes = await _imagenRepository.GetByEmpresaIdAsync(empresa.ID);
            var imagenesPorProducto = todasLasImagenes.GroupBy(i => i.ExternalId ?? 0)
                .ToDictionary(g => g.Key, g => g.OrderBy(i => i.Order).ToList());

            // Obtener imagen de perfil (logo)
            var imagenPerfil = todasLasImagenes.FirstOrDefault(i => i.Type == "PROFILE");
            var logoUrl = imagenPerfil != null && !string.IsNullOrEmpty(imagenPerfil.Urlimagen)
                ? await _imageProcessingService.GetImageUrlAsync(imagenPerfil.Urlimagen)
                : null;

            // Obtener dirección (Sede) de la tabla Direccion - única por empresaID
            var direccion = (await _direccionRepository.GetByEmpresaIdAsync(empresa.ID)).FirstOrDefault();


            var negocioDetalle = new NegocioDetalleDto
            {
                Codigo = negocio.Codigo,
                Nombre = negocio.Nombre ?? "",
                Descripcion = negocio.Descripcion ?? string.Empty,
                Email = "", // Email está en Usuario
                Telefono = empresa.TelefonoPrincipal ?? string.Empty,
                Direccion = direccion?.DireccionTexto ?? "",
                Referencia = direccion?.Referencia ?? "",
                LogoUrl = logoUrl,
                Longitud = direccion?.Longitud ?? 0,
                Latitud = direccion?.Latitud ?? 0,
                Facebook = empresa.Facebook,
                Instagram = empresa.Instagram,
                Twitter = empresa.Twitter,
                Tiktok = empresa.Tiktok,
                Whatsapp = empresa.Whatsapp
            };

            return negocioDetalle;
        }
    }
}






