using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using PedidosBarrio.Application.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace PedidosBarrio.Infrastructure.Services.ImageStrategies
{
    /// <summary>
    /// Estrategia para guardar imágenes de categorías (500x500) con conversión a WebP
    /// </summary>
    public class CategoriaImageSaveStrategy : IImageSaveStrategy
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _categoriaImagePath;
        private readonly string _baseImageUrl;

        public CategoriaImageSaveStrategy(IWebHostEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _categoriaImagePath = Path.Combine(_environment.WebRootPath, "images", "categorias");

            var baseUrl = configuration["BaseUrl"] ?? "https://localhost:7045";
            _baseImageUrl = baseUrl.TrimEnd('/');

            if (_baseImageUrl.EndsWith("/api", StringComparison.OrdinalIgnoreCase))
            {
                _baseImageUrl = _baseImageUrl.Substring(0, _baseImageUrl.Length - 4);
            }

            if (!Directory.Exists(_categoriaImagePath))
            {
                Directory.CreateDirectory(_categoriaImagePath);
            }
        }

        public async Task<string> SaveImageAsync(Stream imageStream, string fileName)
        {
            if (imageStream == null || imageStream.Length == 0)
                throw new ArgumentException("Stream de imagen inválido");

            if (imageStream.Length > 10 * 1024 * 1024)
                throw new ArgumentException("El archivo es demasiado grande. Tamaño máximo: 10MB");

            try
            {
                if (imageStream.CanSeek) imageStream.Position = 0;

                var newFileName = $"{Guid.NewGuid()}_{DateTime.UtcNow:yyyyMMddHHmmss}.webp";
                var filePath = Path.Combine(_categoriaImagePath, newFileName);

                using (var image = await Image.LoadAsync(imageStream))
                {
                    // Redimensionar a 500x500 para categorías
                    image.Mutate(x => x.Resize(500, 500));

                    var encoder = new WebpEncoder
                    {
                        Quality = 75,
                        Method = WebpEncodingMethod.BestQuality
                    };

                    await image.SaveAsync(filePath, encoder);
                }

                var relativeUrl = $"/images/categorias/{newFileName}";
                return $"{_baseImageUrl}{relativeUrl}";
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al procesar imagen de categoría: {ex.Message}", ex);
            }
        }
    }
}
