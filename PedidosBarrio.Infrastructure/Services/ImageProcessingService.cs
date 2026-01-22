using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PedidosBarrio.Application.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace PedidosBarrio.Infrastructure.Services
{
    public class ImageProcessingService : IImageProcessingService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _baseImagePath;
        private readonly string _baseImageUrl;

        public ImageProcessingService(IWebHostEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _baseImagePath = Path.Combine(_environment.WebRootPath, "images", "productos");
            _baseImageUrl = configuration["BaseUrl"] ?? "https://localhost:7045";

            // Crear directorio si no existe
            if (!Directory.Exists(_baseImagePath))
            {
                Directory.CreateDirectory(_baseImagePath);
            }
        }

        public async Task<string> OptimizeAndSaveImageAsync(Stream imageStream, string fileName, int productoId, Guid empresaId)
        {
            if (imageStream == null || imageStream.Length == 0)
                throw new ArgumentException("Stream de imagen inválido");

            // Validar tamaño (máximo 10MB)
            if (imageStream.Length > 10 * 1024 * 1024)
                throw new ArgumentException("El archivo es demasiado grande. Tamaño máximo: 10MB");

            // Generar nombre único con extensión .webp
            var newFileName = $"{empresaId}_{productoId}_{DateTime.UtcNow:yyyyMMddHHmmss}.webp";
            var filePath = Path.Combine(_baseImagePath, newFileName);

            try
            {
                // Reset stream position if possible
                if (imageStream.CanSeek) imageStream.Position = 0;

                using (var image = await Image.LoadAsync(imageStream))
                {
                    // Redimensionar a 200x200
                    image.Mutate(x => x.Resize(200, 200));

                    // Configuramos el encoder de WebP para optimizar y comprimir
                    var encoder = new WebpEncoder
                    {
                        Quality = 75, // Balance ideal entre calidad y peso
                        Method = WebpEncodingMethod.BestQuality
                    };

                    await image.SaveAsync(filePath, encoder);
                }

                // Retornar URL relativa
                return $"/images/productos/{newFileName}";
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al procesar imagen: {ex.Message}", ex);
            }
        }

        public async Task<string> OptimizeAndSaveImageFromUrlAsync(string imageUrl, int productoId, Guid empresaId)
        {
            if (string.IsNullOrEmpty(imageUrl))
                throw new ArgumentException("URL de imagen inválida");

            try
            {
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(30);

                using var response = await httpClient.GetAsync(imageUrl);
                response.EnsureSuccessStatusCode();

                using var imageStream = await response.Content.ReadAsStreamAsync();

                // Generar nombre único con extensión .webp
                var fileName = $"{empresaId}_{productoId}_{DateTime.UtcNow:yyyyMMddHHmmss}.webp";
                var filePath = Path.Combine(_baseImagePath, fileName);

                using (var image = await Image.LoadAsync(imageStream))
                {
                    // Redimensionar a 200x200
                    image.Mutate(x => x.Resize(200, 200));

                    var encoder = new WebpEncoder
                    {
                        Quality = 75,
                        Method = WebpEncodingMethod.BestQuality
                    };

                    await image.SaveAsync(filePath, encoder);
                }

                // Retornar URL relativa
                return $"/images/productos/{fileName}";
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al procesar imagen desde URL: {ex.Message}", ex);
            }
        }

        public Task<bool> DeleteImageAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return Task.FromResult(false);

                // Convertir URL relativa a path físico
                var physicalPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                if (File.Exists(physicalPath))
                {
                    File.Delete(physicalPath);
                    return Task.FromResult(true);
                }

                return Task.FromResult(false);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public Task<string> GetImageUrlAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return Task.FromResult(string.Empty);

            // Si ya es una URL completa, devolverla
            if (filePath.StartsWith("http"))
                return Task.FromResult(filePath);

            // Si es un path relativo, construir URL completa
            var fullUrl = $"{_baseImageUrl.TrimEnd('/')}{filePath}";
            return Task.FromResult(fullUrl);
        }
    }
}