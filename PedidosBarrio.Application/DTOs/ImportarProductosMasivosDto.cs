namespace PedidosBarrio.Application.DTOs
{
    /// <summary>
    /// Respuesta de la importación masiva de productos
    /// </summary>
    public class ImportarProductosMasivosResponseDto
    {
        public int ProductosCreados { get; set; }
        public int ProductosActualizados { get; set; }
        public int PresentacionesCreadas { get; set; }
        public int OpcionesAgregadas { get; set; }
        public int OpcionesActualizadas { get; set; }
        public List<int> ProductosProcesados { get; set; } = new();
        public List<string> Errores { get; set; } = new();
        public bool Exitoso => !Errores.Any() && (ProductosCreados > 0 || ProductosActualizados > 0);
    }
}
