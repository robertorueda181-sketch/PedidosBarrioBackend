namespace PedidosBarrio.Application.Services
{
    /// <summary>
    /// Tipos de imágenes soportados
    /// </summary>
    public enum ImageType
    {
        Banner,
        Producto,
        Empresa
    }

    /// <summary>
    /// Factory para crear estrategias de guardado de imágenes según el tipo
    /// </summary>
    public interface IImageSaveStrategyFactory
    {
        /// <summary>
        /// Obtiene la estrategia de guardado para un tipo de imagen específico
        /// </summary>
        IImageSaveStrategy GetStrategy(ImageType imageType);
    }
}
