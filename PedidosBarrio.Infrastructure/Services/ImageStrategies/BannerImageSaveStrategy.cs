using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using PedidosBarrio.Application.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace PedidosBarrio.Infrastructure.Services.ImageStrategies
{
    /// <summary>
    /// Estrategia para guardar imágenes de banners convertidas a WebP sin redimensionar
    /// </summary>
    public class BannerImageSaveStrategy : IImageSaveStrategy
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _bannerImagePath;
        private readonly string _baseImageUrl;

        public BannerImageSaveStrategy(IWebHostEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _bannerImagePath = Path.Combine(_environment.WebRootPath, "images", "banners");

            var baseUrl = configuration["BaseUrl"] ?? "https://localhost:7045";
            _baseImageUrl = baseUrl.TrimEnd('/');

            // Si la URL base termina en /api, la quitamos
            if (_baseImageUrl.EndsWith("/api", StringComparison.OrdinalIgnoreCase))
            {
                _baseImageUrl = _baseImageUrl.Substring(0, _baseImageUrl.Length - 4);
            }

            // Crear directorio si no existe
            if (!Directory.Exists(_bannerImagePath))
            {
                Directory.CreateDirectory(_bannerImagePath);
            }
        }

        public async Task<string> SaveImageAsync(Stream imageStream, string fileName)
        {
            if (imageStream == null || imageStream.Length == 0)
                throw new ArgumentException("Stream de imagen inválido");

            // Validar tamaño (máximo 10MB)
            if (imageStream.Length > 10 * 1024 * 1024)
                throw new ArgumentException("El archivo es demasiado grande. Tamaño máximo: 10MB");

            // Validar extensión
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(fileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
                throw new ArgumentException($"Formato no permitido. Extensiones permitidas: {string.Join(", ", allowedExtensions)}");

            try
            {
                // Reset stream position if possible
                if (imageStream.CanSeek) imageStream.Position = 0;

                // Generar nombre único con extensión .webp
                var newFileName = $"{Guid.NewGuid()}_{DateTime.UtcNow:yyyyMMddHHmmss}.webp";
                var filePath = Path.Combine(_bannerImagePath, newFileName);

                using (var image = await Image.LoadAsync(imageStream))
                {
                    // NO redimensionamos banners, pero convertimos a WebP
                    var encoder = new WebpEncoder
                    {
                        Quality = 80, // Calidad para banners (más alta que productos)
                        Method = WebpEncodingMethod.BestQuality
                    };

                    await image.SaveAsync(filePath, encoder);
                }

                // Retornar URL completa con baseURL
                var relativeUrl = $"/images/banners/{newFileName}";
                return $"{_baseImageUrl}{relativeUrl}";
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al guardar imagen de banner: {ex.Message}", ex);
            }
        }
    }
}
