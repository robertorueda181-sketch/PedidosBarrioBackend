using MediatR;
using Microsoft.AspNetCore.Http;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.UploadEmpresaProfileImage
{
    public class UploadEmpresaProfileImageCommand : IRequest<UploadEmpresaLogoResponseDto>
    {
        public Guid EmpresaId { get; set; }
        public IFormFile FileStream { get; set; }
        public string FileName { get; set; }

        public UploadEmpresaProfileImageCommand(Guid empresaId, IFormFile fileStream, string fileName)
        {
            EmpresaId = empresaId;
            FileStream = fileStream;
            FileName = fileName;
        }
    }
}
