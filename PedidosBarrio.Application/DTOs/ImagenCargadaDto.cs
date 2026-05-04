namespace PedidosBarrio.Application.DTOs
{
    public class ImagenCargadaDto
    {
        public string File { get; set; } = string.Empty;
        public string ProductoCodigo { get; set; } = string.Empty;
        public int ProductoId { get; set; }
        public string OpcionDescripcion { get; set; } = string.Empty;
        public int OpcionId { get; set; }
        public string ImagenUrl { get; set; } = string.Empty;
        public string Estado { get; set; } = "OK";
    }
}
