namespace PedidosBarrio.Application.DTOs
{
    /// <summary>
    /// DTO que devuelve detalles de un negocio (empresa) con sus productos y categorías
    /// </summary>
    public class NegocioDetalleDto
    {
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Referencia { get; set; }
        public string? LogoUrl { get; set; }
        public decimal? Longitud { get; set; }
        public decimal? Latitud { get; set; }
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public string? Twitter { get; set; }
        public string? Tiktok { get; set; }
        public string? Whatsapp { get; set; }
    }
}

