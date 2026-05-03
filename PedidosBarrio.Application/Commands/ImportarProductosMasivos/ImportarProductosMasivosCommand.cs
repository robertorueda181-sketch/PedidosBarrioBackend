using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.ImportarProductosMasivos
{
    /// <summary>
    /// Comando para importar productos de forma masiva desde un archivo Excel cargado
    /// </summary>
    public class ImportarProductosMasivosCommand : IRequest<ImportarProductosMasivosResponseDto>
    {
        public Stream ArchivoStream { get; }
        public string NombreArchivo { get; }
        public Guid EmpresaId { get; }

        public ImportarProductosMasivosCommand(Stream archivoStream, string nombreArchivo, Guid empresaId)
        {
            ArchivoStream = archivoStream ?? throw new ArgumentNullException(nameof(archivoStream));
            NombreArchivo = nombreArchivo ?? throw new ArgumentNullException(nameof(nombreArchivo));
            EmpresaId = empresaId != Guid.Empty ? empresaId : throw new ArgumentException("EmpresaId no puede estar vacío", nameof(empresaId));
        }
    }
}
