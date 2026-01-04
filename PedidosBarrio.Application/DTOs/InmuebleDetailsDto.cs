namespace PedidosBarrio.Application.DTOs
{
    /// <summary>
    /// DTO para devolver Inmueble con datos completos
    /// Incluye: Tipo del Inmueble, Operación (tipo de transacción) e Imágenes
    /// Se usa en queries que hacen JOINs con Tipos e Imagenes
    /// </summary>
    public class InmuebleDetailsDto
    {
        public int InmuebleID { get; set; }
        public Guid EmpresaID { get; set; }
        
        // Tipo del Inmueble (Casa, Departamento, etc.)
        public int TiposID { get; set; }
        public string TipoInmueble { get; set; }
        
        // Operación (Venta, Alquiler, etc.)
        public int? OperacionID { get; set; }
        public string TipoOperacion { get; set; }
        
        public decimal Precio { get; set; }
        public string Medidas { get; set; }
        public string Ubicacion { get; set; }
        public int Dormitorios { get; set; }
        public int Banos { get; set; }
        public string Descripcion { get; set; }
        
        public List<ImagenUrlDto> Imagenes { get; set; } = new List<ImagenUrlDto>();
    }
}
