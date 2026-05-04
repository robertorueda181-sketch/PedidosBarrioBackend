using Microsoft.AspNetCore.Http;

namespace PedidosBarrio.Application.DTOs
{
    public class ArchivoImagenDto
    {
        public IFormFile Stream { get; set; } = null!;
        public string FileName { get; set; } = string.Empty;
        public long Length { get; set; }
    }
}
