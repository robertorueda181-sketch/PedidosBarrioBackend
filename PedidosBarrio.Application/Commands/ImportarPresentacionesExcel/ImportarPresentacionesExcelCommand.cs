using MediatR;

namespace PedidosBarrio.Application.Commands.ImportarPresentacionesExcel
{
    public class ImportarPresentacionesExcelCommand : IRequest<ImportarPresentacionesExcelResult>
    {
        public Stream ExcelStream { get; }
        public string FileName { get; }

        public ImportarPresentacionesExcelCommand(Stream excelStream, string fileName)
        {
            ExcelStream = excelStream ?? throw new ArgumentNullException(nameof(excelStream));
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        }
    }

    public class ImportarPresentacionesExcelResult
    {
        public int ProductosCreados { get; set; }
        public int ProductosActualizados { get; set; }
        public int PresentacionesCreadas { get; set; }
        public int OpcionesAgregadas { get; set; }
        public int OpcionesActualizadas { get; set; }
        public List<int> ProductosProcesados { get; set; } = new();
        public List<string> Errores { get; set; } = new();
    }
}
