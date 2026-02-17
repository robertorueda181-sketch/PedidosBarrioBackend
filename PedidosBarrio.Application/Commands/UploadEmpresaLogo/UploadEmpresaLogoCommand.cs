using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.UploadEmpresaLogo
{
    public class UploadEmpresaLogoCommand : IRequest<UploadEmpresaLogoResponseDto>
    {
        public Guid EmpresaId { get; set; }
        public Stream FileStream { get; set; }
        public string FileName { get; set; }

        public UploadEmpresaLogoCommand(Guid empresaId, Stream fileStream, string fileName)
        {
            EmpresaId = empresaId;
            FileStream = fileStream;
            FileName = fileName;
        }
    }
}
