using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.CreateBanner
{
    public class CreateBannerWithImageCommandHandler : IRequestHandler<CreateBannerWithImageCommand, BannerResponseDto>
    {
        private readonly IBannerRepository _bannerRepository;
        private readonly IImageSaveStrategyFactory _imageSaveStrategyFactory;
        private readonly IApplicationLogger _logger;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const long MaxFileSizeMB = 10;

        public CreateBannerWithImageCommandHandler(
            IBannerRepository bannerRepository,
            IImageSaveStrategyFactory imageSaveStrategyFactory,
            IApplicationLogger logger)
        {
            _bannerRepository = bannerRepository;
            _imageSaveStrategyFactory = imageSaveStrategyFactory;
            _logger = logger;
        }

        public async Task<BannerResponseDto> Handle(CreateBannerWithImageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string? urlImagen = null;

                // Procesar imagen si se proporciona
                if (request.ImagenStream != null && request.ImagenFileName != null)
                {
                    // Validar extensión
                    var fileExtension = Path.GetExtension(request.ImagenFileName).ToLower();
                    if (!_allowedExtensions.Contains(fileExtension))
                    {
                        await _logger.LogWarningAsync($"Extensión no permitida para imagen de banner: {fileExtension}");
                        return new BannerResponseDto
                        {
                            Success = false,
                            Message = $"Formato no permitido. Extensiones permitidas: {string.Join(", ", _allowedExtensions)}"
                        };
                    }

                    // Validar tamaño
                    if (request.ImagenStream.Length > MaxFileSizeMB * 1024 * 1024)
                    {
                        await _logger.LogWarningAsync($"Archivo demasiado grande para imagen de banner: {request.ImagenStream.Length} bytes");
                        return new BannerResponseDto
                        {
                            Success = false,
                            Message = $"El archivo es demasiado grande. Máximo permitido: {MaxFileSizeMB}MB"
                        };
                    }

                    try
                    {
                        // Obtener estrategia de guardado para banners (guarda en /images/banners/ y convierte a WebP)
                        var bannerStrategy = _imageSaveStrategyFactory.GetStrategy(ImageType.Banner);
                        urlImagen = await bannerStrategy.SaveImageAsync(request.ImagenStream, request.ImagenFileName);

                        await _logger.LogInformationAsync($"Imagen de banner guardada: {urlImagen}");
                    }
                    catch (ArgumentException ex)
                    {
                        await _logger.LogWarningAsync($"Validación de imagen fallida: {ex.Message}");
                        return new BannerResponseDto
                        {
                            Success = false,
                            Message = ex.Message
                        };
                    }
                }
                // Si no hay archivo pero hay URL, usar la URL directamente
                else if (!string.IsNullOrEmpty(request.ImagenUrl))
                {
                    urlImagen = request.ImagenUrl;
                    await _logger.LogInformationAsync($"URL de imagen de banner guardada directamente: {urlImagen}");
                }

                // Crear banner
                var banner = new Banner
                {
                    EmpresaID = request.EmpresaID,
                    Titulo = request.Titulo,
                    Descripcion = request.Descripcion,
                    TextoBoton = request.TextoBoton,
                    Link = request.Link,
                    UrlImagen = urlImagen,
                    FechaInicio = request.FechaInicio,
                    FechaExpiracion = request.FechaExpiracion,
                    Visible = request.Visible ?? true,
                    Aprobado = request.Aprobado ?? false,
                    Prioridad = request.Prioridad,
                    FechaCreacion = DateTime.UtcNow
                };

                var bannerId = await _bannerRepository.AddAsync(banner);

                await _logger.LogInformationAsync($"Banner creado exitosamente: {bannerId} para empresa {request.EmpresaID}");

                return new BannerResponseDto
                {
                    Success = true,
                    Message = "Banner creado exitosamente",
                    BannerId = bannerId,
                    UrlImagen = urlImagen
                };
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync($"Error al crear banner: {ex.Message}", ex);
                return new BannerResponseDto
                {
                    Success = false,
                    Message = "Error al crear el banner"
                };
            }
        }
    }
}
