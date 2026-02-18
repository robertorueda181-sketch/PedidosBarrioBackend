namespace PedidosBarrio.Application.DTOs
{
    public class CreateBannerDto
    {
        public Guid EmpresaID { get; set; }
        public string? Titulo { get; set; }
        public string? Descripcion { get; set; } // Subtítulo
        public string? TextoBoton { get; set; }
        public string? Link { get; set; }
        public string? Redireccion { get; set; } // URL de redirección
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime FechaExpiracion { get; set; }
        public bool? Visible { get; set; } = true;
        public bool? Aprobado { get; set; } = false;
        public short Prioridad { get; set; } = 1;
    }

    public class BannerResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int BannerId { get; set; }
        public string? UrlImagen { get; set; }
    }

    public class BannerDetailDto
    {
        public int BannerID { get; set; }
        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
        public string? TextoBoton { get; set; }
        public string? Link { get; set; }
        public string? Redireccion { get; set; }
        public string? UrlImagen { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime FechaExpiracion { get; set; }
        public bool? Visible { get; set; }
        public bool? Aprobado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public Guid EmpresaID { get; set; }
        public short Prioridad { get; set; }
    }
}
