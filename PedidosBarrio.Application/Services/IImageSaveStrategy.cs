namespace PedidosBarrio.Application.Services
{
    /// <summary>
    /// Define las estrategias de procesamiento de imágenes por tipo
    /// </summary>
    public interface IImageSaveStrategy
    {
        /// <summary>
        /// Guarda la imagen usando la estrategia específica
        /// </summary>
        Task<string> SaveImageAsync(Stream imageStream, string fileName);
    }
}
