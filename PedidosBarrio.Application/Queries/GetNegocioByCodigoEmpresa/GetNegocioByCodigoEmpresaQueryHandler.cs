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
        private readonly IImageProcessingService _imageProcessingService;
        private readonly IMapper _mapper;

        public GetNegocioByCodigoEmpresaQueryHandler(
            IEmpresaRepository empresaRepository,
            INegocioRepository negocioRepository,
            IProductoRepository productoRepository,
            ICategoriaRepository categoriaRepository,
            IImagenRepository imagenRepository,
            IImageProcessingService imageProcessingService,
            IMapper mapper)
        {
            _empresaRepository = empresaRepository;
            _negocioRepository = negocioRepository;
            _productoRepository = productoRepository;
            _categoriaRepository = categoriaRepository;
            _imagenRepository = imagenRepository;
            _imageProcessingService = imageProcessingService;
            _mapper = mapper;
        }

        public async Task<NegocioDetalleDto> Handle(GetNegocioByCodigoEmpresaQuery query, CancellationToken cancellationToken)
        {
            var empresa = await _negocioRepository.GetByCodigoEmpresaAsync(query.CodigoEmpresa);

            if (empresa == null)
                return null;

            // Obtener productos de la empresa
            var productos = (await _productoRepository.GetByEmpresaIdAsync(empresa.ID)).ToList();

            // Obtener categorías que deben mostrarse (Mostrar = true y Activo = true)
            var categorias = await _categoriaRepository.GetByEmpresaIdMostrandoAsync(empresa.ID);

            // Obtener imágenes de la empresa para los productos
            var todasLasImagenes = await _imagenRepository.GetByEmpresaIdAsync(empresa.ID);
            var imagenesPorProducto = todasLasImagenes.GroupBy(i => i.ExternalId ?? 0)
                .ToDictionary(g => g.Key, g => g.OrderBy(i => i.Order).ToList());

            var productoDtos = new List<ProductoDetalleDto>();

            foreach (var p in productos)
            {
                var dto = new ProductoDetalleDto
                {
                    ProductoID = p.ProductoID,
                    CategoriaID = p.CategoriaID ?? 0,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion ?? string.Empty,
                    Stock = p.Stock,
                    Precios = p.Presentaciones.SelectMany(pres => pres.Precios)
                        .Select(pre => _mapper.Map<PrecioDto>(pre)).ToList()
                };

                // Asignar precio principal
                var principal = dto.Precios.FirstOrDefault(pre => pre.EsPrincipal) ?? dto.Precios.FirstOrDefault();
                dto.Precio = principal?.PrecioValor ?? 0;

                // Asignar imagen
                if (imagenesPorProducto.TryGetValue(p.ProductoID, out var imgs) && imgs.Any())
                {
                    var firstImg = imgs.First();
                    dto.URLImagen = !string.IsNullOrEmpty(firstImg.Urlimagen) 
                        ? await _imageProcessingService.GetImageUrlAsync(firstImg.Urlimagen) 
                        : string.Empty;
                }

                productoDtos.Add(dto);
            }

            var negocioDetalle = new NegocioDetalleDto
            {
                EmpresaID = empresa.ID,
                Nombre = empresa.Nombre,
                Descripcion = empresa.Descripcion,
                Email = empresa.Email,
                Telefono = empresa.Telefono,
                Direccion = empresa.Direccion,
                Referencia = empresa.Referencia ?? string.Empty,
                Categorias = categorias.Select(c => new CategoriaDetalleDto
                {
                    CategoriaID = c.CategoriaID,
                    Descripcion = c.Descripcion,
                    Codigo = c.Color ?? string.Empty,
                    Mostrar = c.Activa ?? false
                }).ToList(),
                Productos = productoDtos
            };

            return negocioDetalle;
        }
    }
}




