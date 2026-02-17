namespace PedidosBarrio.Application.DTOs
{
    public class UploadEmpresaLogoResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ImagePath { get; set; }
        public string ImageUrl { get; set; }
    }
}
