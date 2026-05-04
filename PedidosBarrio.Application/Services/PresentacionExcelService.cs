using ClosedXML.Excel;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Services
{
    public interface IPresentacionExcelService
    {
        Task<List<PresentacionExcelRowDto>> LeerPresentacionesAsync(Stream excelStream);
    }

    public class PresentacionExcelService : IPresentacionExcelService
    {
        public async Task<List<PresentacionExcelRowDto>> LeerPresentacionesAsync(Stream excelStream)
        {
            var result = new List<PresentacionExcelRowDto>();
            using var wb = new XLWorkbook(excelStream);
            var ws = wb.Worksheet("Catalogo");
            int lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;

            bool? ParseBool(string raw)
            {
                if (string.IsNullOrWhiteSpace(raw)) return null;
                var v = raw.Trim().ToLowerInvariant();
                return v is "true" or "1" or "si" or "sí" or "s" ? true :
                       v is "false" or "0" or "no" or "n" ? false : null;
            }

            for (int row = 3; row <= lastRow; row++)
            {
                var codigo = ws.Cell(row, 1).GetValue<string>();
                var catRaw = ws.Cell(row, 2).GetValue<string>();
                var nombre = ws.Cell(row, 3).GetValue<string>();

                if (string.IsNullOrWhiteSpace(catRaw) && string.IsNullOrWhiteSpace(nombre) && string.IsNullOrWhiteSpace(codigo))
                    continue;

                bool stockMinOk = int.TryParse(ws.Cell(row, 5).GetValue<string>(), out int stockMinVal);
                bool precioOk = decimal.TryParse(ws.Cell(row, 7).GetValue<string>(), out decimal precioVal);
                result.Add(new PresentacionExcelRowDto
                {
                    ExcelRow = row,
                    Codigo = string.IsNullOrWhiteSpace(codigo) ? null : codigo.Trim(),
                    Categoria = string.IsNullOrWhiteSpace(catRaw) ? null : catRaw.Trim(),
                    NombreProducto = string.IsNullOrWhiteSpace(nombre) ? null : nombre,
                    DescripcionProducto = ws.Cell(row, 4).GetValue<string>(),
                    StockMinimo = stockMinOk ? stockMinVal : null,
                    Visible = ParseBool(ws.Cell(row,6).GetValue<string>()),
                    Precio = precioOk ? precioVal : null,
                    NombrePresentacion1 = ws.Cell(row, 8).GetValue<string>(),
                    DescripcionOpcion1 = ws.Cell(row, 9).GetValue<string>(),
                    NombrePresentacion2 = ws.Cell(row, 10).GetValue<string>(),
                    DescripcionOpcion2 = ws.Cell(row, 11).GetValue<string>(),
                    NombrePresentacion3 = ws.Cell(row, 12).GetValue<string>(),
                    DescripcionOpcion3 = ws.Cell(row, 13).GetValue<string>()
                });
            }

            return result;
        }
    }
}
