namespace PedidosBarrio.Application.DTOs
{
    /// <summary>
    /// DTO para importar presentaciones y opciones desde Excel
    /// </summary>
    public class PresentacionExcelRowDto
    {
        /// <summary>
        /// ID del producto
        /// </summary>
        public int ProductoID { get; set; }

        /// <summary>
        /// Nombre de la presentación (ej: "Talla", "Color")
        /// </summary>
        public string NombrePresentacion { get; set; } = null!;

        /// <summary>
        /// Valor de la opción (ej: "S", "M", "L" o "Rojo", "Verde", "Azul")
        /// </summary>
        public string ValorOpcion { get; set; } = null!;

        /// <summary>
        /// Precio específico para esta opción
        /// </summary>
        public decimal? PrecioOpcion { get; set; }

        /// <summary>
        /// URL de la imagen para esta opción
        /// </summary>
        public string? ImagenOpcion { get; set; }

        /// <summary>
        /// Stock disponible para esta opción
        /// </summary>
        public int? StockOpcion { get; set; }

        /// <summary>
        /// Descripción de la opción
        /// </summary>
        public string? DescripcionOpcion { get; set; }
    }
}
