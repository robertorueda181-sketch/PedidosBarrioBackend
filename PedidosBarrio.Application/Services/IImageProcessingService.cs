namespace PedidosBarrio.Application.Services
{
    public interface IImageProcessingService
    {
        Task<string> OptimizeAndSaveImageAsync(Stream imageStream, string fileName, int productoId, Guid empresaId);
        Task<string> OptimizeAndSaveImageFromUrlAsync(string imageUrl, int productoId, Guid empresaId);
        Task<bool> DeleteImageAsync(string filePath);
        Task<string> GetImageUrlAsync(string filePath);
    }
}