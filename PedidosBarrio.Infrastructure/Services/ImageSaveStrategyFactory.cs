using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Infrastructure.Services.ImageStrategies;

namespace PedidosBarrio.Infrastructure.Services
{
    /// <summary>
    /// Factory que crea estrategias de guardado de imágenes según el tipo
    /// </summary>
    public class ImageSaveStrategyFactory : IImageSaveStrategyFactory
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public ImageSaveStrategyFactory(IWebHostEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
        }

        public IImageSaveStrategy GetStrategy(ImageType imageType)
        {
            return imageType switch
            {
                ImageType.Banner => new BannerImageSaveStrategy(_environment, _configuration),
                ImageType.Producto => new ProductoImageSaveStrategy(_environment, _configuration),
                ImageType.Empresa => new ProductoImageSaveStrategy(_environment, _configuration), // Por ahora usa la misma que producto
                _ => throw new ArgumentException($"Tipo de imagen no soportado: {imageType}")
            };
        }
    }
}
