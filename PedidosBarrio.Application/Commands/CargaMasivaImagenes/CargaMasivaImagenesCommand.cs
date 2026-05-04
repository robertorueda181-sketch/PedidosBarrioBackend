using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.CargaMasivaImagenes
{
    public class CargaMasivaImagenesCommand : IRequest<CargaMasivaImagenesResponseDto>
    {
        public IEnumerable<ArchivoImagenDto> Imagenes { get; }
        public Guid EmpresaId { get; }

        public CargaMasivaImagenesCommand(IEnumerable<ArchivoImagenDto> imagenes, Guid empresaId)
        {
            Imagenes = imagenes ?? throw new ArgumentNullException(nameof(imagenes));
            EmpresaId = empresaId != Guid.Empty ? empresaId : throw new ArgumentException("EmpresaId no puede estar vacío", nameof(empresaId));
        }
    }
}
