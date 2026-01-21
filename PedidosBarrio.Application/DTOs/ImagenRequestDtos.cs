namespace PedidosBarrio.Application.DTOs
{
    public class UploadImageRequestDto
    {
        public int ProductoId { get; set; }
        public string Descripcion { get; set; }
        public bool SetAsPrincipal { get; set; } = false;
    }

    public class UploadImageFromUrlRequestDto
    {
        public int ProductoId { get; set; }
        public string ImageUrl { get; set; }
        public string Descripcion { get; set; }
        public bool SetAsPrincipal { get; set; } = false;
    }

    public class UpdateImageRequestDto
    {
        public int ImagenId { get; set; }
        public string Descripcion { get; set; }
        public bool SetAsPrincipal { get; set; } = false;
    }

    public class ImageResponseDto
    {
        public int ImagenID { get; set; }
        public string URLImagen { get; set; }
        public string URLCompleta { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public short Order { get; set; }
        public bool IsPrincipal { get; set; }
    }
}