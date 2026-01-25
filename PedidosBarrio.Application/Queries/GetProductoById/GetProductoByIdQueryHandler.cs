using AutoMapper;
using MediatR;
using System.Linq;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetProductoById
{
    public class GetProductoByIdQueryHandler : IRequestHandler<GetProductoByIdQuery, ProductoDto>
    {
        private readonly IProductoRepository _productoRepository;
        private readonly IImagenRepository _imagenRepository;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetProductoByIdQueryHandler(
            IProductoRepository productoRepository, 
            IImagenRepository imagenRepository,
            IImageProcessingService imageProcessingService,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _productoRepository = productoRepository;
            _imagenRepository = imagenRepository;
            _imageProcessingService = imageProcessingService;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<ProductoDto> Handle(GetProductoByIdQuery query, CancellationToken cancellationToken)
        {
            // If the user is not logged in, this will fail by design
            var empresaId = _currentUserService.GetEmpresaId();

            var producto = await _productoRepository.GetByIdAsync(query.ProductoID, empresaId);
            if (producto == null)
            {
                return null;
            }

            var dto = _mapper.Map<ProductoDto>(producto);

            // Obtener imágenes
            var imagenes = await _imagenRepository.GetByProductoIdAsync(query.ProductoID);
            foreach (var img in imagenes)
            {
                var imgDto = new ImagenProductoDto
                {
                    ImagenID = img.ImagenID,
                    ExternalId = img.ExternalId ?? 0,
                    URLImagen = img.Urlimagen,
                    Descripcion = img.Descripcion ?? string.Empty,
                    FechaRegistro = img.FechaRegistro ?? DateTime.Now,
                    Activa = img.Activa,
                    Type = img.Type ?? "PRODUCT",
                    Order = img.Order,
                    EmpresaID = img.EmpresaID ?? Guid.Empty
                };

                if (!string.IsNullOrEmpty(imgDto.URLImagen))
                {
                    imgDto.URLImagen = await _imageProcessingService.GetImageUrlAsync(imgDto.URLImagen);
                }

                dto.Imagenes.Add(imgDto);
            }

            // Imagen principal
            var principal = dto.Imagenes.OrderBy(i => i.Order).FirstOrDefault();
            if (principal != null)
            {
                dto.ImagenPrincipal = principal.URLImagen;
            }

            // Calcular PrecioActual si no viene mapeado
            if (dto.PrecioActual == null && dto.Precios.Any())
            {
                dto.PrecioActual = dto.Precios.FirstOrDefault(p => p.EsPrincipal)?.PrecioValor 
                                   ?? dto.Precios.FirstOrDefault()?.PrecioValor;
            }

            return dto;
        }
    }
}
