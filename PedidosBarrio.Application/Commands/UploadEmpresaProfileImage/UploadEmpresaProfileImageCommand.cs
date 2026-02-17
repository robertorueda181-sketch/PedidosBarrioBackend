using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.UploadEmpresaProfileImage
{
    public class UploadEmpresaProfileImageCommand : IRequest<UploadEmpresaLogoResponseDto>
    {
        public Guid EmpresaId { get; set; }
        public Stream FileStream { get; set; }
        public string FileName { get; set; }

        public UploadEmpresaProfileImageCommand(Guid empresaId, Stream fileStream, string fileName)
        {
            EmpresaId = empresaId;
            FileStream = fileStream;
            FileName = fileName;
        }
    }
}
