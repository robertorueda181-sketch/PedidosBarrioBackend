namespace PedidosBarrio.Application.Services
{
    /// <summary>
    /// Tipos de imágenes soportados con sus dimensiones específicas
    /// </summary>
    public enum ImageType
    {
        /// <summary>Banner - 1200x600</summary>
        Banner,
        /// <summary>Producto - 400x400</summary>
        Producto,
        /// <summary>Empresa/Logo - 300x300</summary>
        Empresa,
        /// <summary>Categoría - 500x500</summary>
        Categoria,
        /// <summary>Avatar/Perfil - 200x200</summary>
        Avatar,
        /// <summary>Sin ajuste - solo convierte a WebP</summary>
        Original
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
