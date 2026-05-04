namespace PedidosBarrio.Application.DTOs
{
    /// <summary>
    /// DTO para importar presentaciones y opciones desde Excel
    /// </summary>
    public class PresentacionExcelRowDto
    {
        public int ExcelRow { get; set; }
        public string? Codigo { get; set; }
        public string? Categoria { get; set; } // Descripcion de la categoria (en lugar de CategoriaID)
        public string? NombreProducto { get; set; }
        public string? DescripcionProducto { get; set; }
        public int? StockMinimo { get; set; }
        public bool? Visible { get; set; }

        public decimal? Precio { get; set; }
        public string? NombrePresentacion1 { get; set; }
        public string? DescripcionOpcion1 { get; set; }
        public string? NombrePresentacion2 { get; set; }
        public string? DescripcionOpcion2 { get; set; }
        public string? NombrePresentacion3 { get; set; }
        public string? DescripcionOpcion3 { get; set; }
    }

}
