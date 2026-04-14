using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using PedidosBarrio.Application.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace PedidosBarrio.Infrastructure.Services.ImageStrategies
{
    /// <summary>
    /// Estrategia para guardar imágenes de productos (400x400) con conversión a WebP
    /// </summary>
    public class ProductoImageSaveStrategy : IImageSaveStrategy
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _productoImagePath;
        private readonly string _baseImageUrl;

        public ProductoImageSaveStrategy(IWebHostEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _productoImagePath = Path.Combine(_environment.WebRootPath, "images", "productos");

            var baseUrl = configuration["BaseUrl"] ?? "https://localhost:7045";
            _baseImageUrl = baseUrl.TrimEnd('/');

            // Si la URL base termina en /api, la quitamos
            if (_baseImageUrl.EndsWith("/api", StringComparison.OrdinalIgnoreCase))
            {
                _baseImageUrl = _baseImageUrl.Substring(0, _baseImageUrl.Length - 4);
            }

            // Crear directorio si no existe
            if (!Directory.Exists(_productoImagePath))
            {
                Directory.CreateDirectory(_productoImagePath);
            }
        }

        public async Task<string> SaveImageAsync(Stream imageStream, string fileName)
        {
            if (imageStream == null || imageStream.Length == 0)
                throw new ArgumentException("Stream de imagen inválido");

            // Validar tamaño (máximo 10MB)
            if (imageStream.Length > 10 * 1024 * 1024)
                throw new ArgumentException("El archivo es demasiado grande. Tamaño máximo: 10MB");

            try
            {
                // Reset stream position if possible
                if (imageStream.CanSeek) imageStream.Position = 0;

                // Generar nombre único con extensión .webp
                var newFileName = $"{Guid.NewGuid()}_{DateTime.UtcNow:yyyyMMddHHmmss}.webp";
                var filePath = Path.Combine(_productoImagePath, newFileName);

                using (var image = await Image.LoadAsync(imageStream))
                {
                    // Redimensionar a 400x400 para productos
                    image.Mutate(x => x.Resize(400, 400));

                    // Configuramos el encoder de WebP para optimizar y comprimir
                    var encoder = new WebpEncoder
                    {
                        Quality = 75, // Balance ideal entre calidad y peso
                        Method = WebpEncodingMethod.BestQuality
                    };

                    await image.SaveAsync(filePath, encoder);
                }

                // Retornar URL completa con baseURL
                var relativeUrl = $"/images/productos/{newFileName}";
                return $"{_baseImageUrl}{relativeUrl}";
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al procesar imagen de producto: {ex.Message}", ex);
            }
        }
    }
}
