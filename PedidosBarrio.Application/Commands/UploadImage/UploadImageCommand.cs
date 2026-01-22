using System.IO;
using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.UploadImage
{
    public class UploadImageCommand : IRequest<ImageResponseDto>
    {
        public int ProductoId { get; set; }
        public string? Descripcion { get; set; }
        public bool SetAsPrincipal { get; set; }
        public Stream FileStream { get; set; }
        public string FileName { get; set; }

        public UploadImageCommand(int productoId, string? descripcion, bool setAsPrincipal, Stream fileStream, string fileName)
        {
            ProductoId = productoId;
            Descripcion = descripcion;
            SetAsPrincipal = setAsPrincipal;
            FileStream = fileStream;
            FileName = fileName;
        }
    }
}
