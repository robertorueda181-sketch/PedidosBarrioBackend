namespace PedidosBarrio.Application.DTOs
{
    /// <summary>
    /// DTO para devolver un Inmueble con sus imagenes asociadas
    /// Se usa en queries que hacen INNER/LEFT JOIN con la tabla Imagenes
    /// </summary>
    public class InmuebleWithImagesDto
    {
        public int InmuebleID { get; set; }
        public Guid EmpresaID { get; set; }
        public int TiposID { get; set; }
        public string Tipo { get; set; }
        public decimal Precio { get; set; }
        public string Medidas { get; set; }
        public string Ubicacion { get; set; }
        public int Dormitorios { get; set; }
        public int Banos { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaRegistro { get; set; }
        
        /// <summary>
        /// Lista de imagenes del inmueble
        /// Se popula cuando se hace JOIN con tabla Imagenes
        /// </summary>
        public List<ImagenUrlDto> Imagenes { get; set; } = new List<ImagenUrlDto>();
    }
}
