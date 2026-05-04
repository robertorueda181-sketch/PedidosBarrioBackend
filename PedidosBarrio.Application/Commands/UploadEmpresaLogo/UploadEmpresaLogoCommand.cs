using MediatR;
using Microsoft.AspNetCore.Http;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.UploadEmpresaLogo
{
    public class UploadEmpresaLogoCommand : IRequest<UploadEmpresaLogoResponseDto>
    {
        public Guid EmpresaId { get; set; }
        public IFormFile FileStream { get; set; }
        public string FileName { get; set; }

        public UploadEmpresaLogoCommand(Guid empresaId, IFormFile fileStream, string fileName)
        {
            EmpresaId = empresaId;
            FileStream = fileStream;
            FileName = fileName;
        }
    }
}
