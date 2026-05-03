using ClosedXML.Excel;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Services
{
    public interface IPresentacionExcelService
    {
        /// <summary>
        /// Genera una plantilla Excel para importar presentaciones
        /// </summary>
        Task<byte[]> GenerarPlantillaAsync();

        /// <summary>
        /// Lee presentaciones desde un archivo Excel
        /// </summary>
        Task<List<PresentacionExcelRowDto>> LeerPresentacionesAsync(Stream excelStream);
    }

    public class PresentacionExcelService : IPresentacionExcelService
    {
        public async Task<byte[]> GenerarPlantillaAsync()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Presentaciones");

                // Encabezados
                worksheet.Cell(1, 1).Value = "ProductoID*";
                worksheet.Cell(1, 2).Value = "NombrePresentacion*";
                worksheet.Cell(1, 3).Value = "ValorOpcion*";
                worksheet.Cell(1, 4).Value = "PrecioOpcion";
                worksheet.Cell(1, 5).Value = "ImagenOpcion";
                worksheet.Cell(1, 6).Value = "StockOpcion";
                worksheet.Cell(1, 7).Value = "DescripcionOpcion";

                // Formato de encabezados
                var headerRow = worksheet.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.LightBlue;
                headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Agregar filas de ejemplo
                worksheet.Cell(2, 1).Value = 1;
                worksheet.Cell(2, 2).Value = "Talla";
                worksheet.Cell(2, 3).Value = "S";
                worksheet.Cell(2, 4).Value = 25.99;
                worksheet.Cell(2, 5).Value = "https://ejemplo.com/imagen-s.jpg";
                worksheet.Cell(2, 6).Value = 50;
                worksheet.Cell(2, 7).Value = "Talla Pequeña";

                worksheet.Cell(3, 1).Value = 1;
                worksheet.Cell(3, 2).Value = "Talla";
                worksheet.Cell(3, 3).Value = "M";
                worksheet.Cell(3, 4).Value = 29.99;
                worksheet.Cell(3, 5).Value = "https://ejemplo.com/imagen-m.jpg";
                worksheet.Cell(3, 6).Value = 100;
                worksheet.Cell(3, 7).Value = "Talla Mediana";

                worksheet.Cell(4, 1).Value = 1;
                worksheet.Cell(4, 2).Value = "Color";
                worksheet.Cell(4, 3).Value = "Rojo";
                worksheet.Cell(4, 4).Value = "";  // Cambiar null por string vacío
                worksheet.Cell(4, 5).Value = "https://ejemplo.com/imagen-rojo.jpg";
                worksheet.Cell(4, 6).Value = 75;
                worksheet.Cell(4, 7).Value = "Color Rojo";

                // Ancho de columnas
                worksheet.Column(1).Width = 12;
                worksheet.Column(2).Width = 20;
                worksheet.Column(3).Width = 15;
                worksheet.Column(4).Width = 15;
                worksheet.Column(5).Width = 30;
                worksheet.Column(6).Width = 15;
                worksheet.Column(7).Width = 20;

                // Añadir nota de instrucciones
                var notesWorksheet = workbook.Worksheets.Add("Instrucciones");
                notesWorksheet.Cell(1, 1).Value = "Instrucciones para importar presentaciones:";
                notesWorksheet.Cell(2, 1).Value = "1. Completa la columna 'ProductoID' con el ID del producto";
                notesWorksheet.Cell(3, 1).Value = "2. En 'NombrePresentacion' especifica el tipo (Talla, Color, Tamaño, etc.)";
                notesWorksheet.Cell(4, 1).Value = "3. En 'ValorOpcion' especifica la opción (S, M, L, Rojo, Verde, etc.)";
                notesWorksheet.Cell(5, 1).Value = "4. 'PrecioOpcion' es opcional (si no se especifica usa el precio del producto)";
                notesWorksheet.Cell(6, 1).Value = "5. Las columnas marcadas con * son obligatorias";
                notesWorksheet.Cell(7, 1).Value = "6. Para varias presentaciones del mismo producto, agrega filas con presentación diferente";
                notesWorksheet.Cell(8, 1).Value = "7. Ejemplo: ProductoID=1, con Talla(S, M, L) y Color(Rojo, Verde)";

                notesWorksheet.Column(1).Width = 80;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public async Task<List<PresentacionExcelRowDto>> LeerPresentacionesAsync(Stream excelStream)
        {
            var filas = new List<PresentacionExcelRowDto>();

            using (var workbook = new XLWorkbook(excelStream))
            {
                var worksheet = workbook.Worksheet("Presentaciones");

                int rowCount = worksheet.RowsUsed().Count();

                // Empezar desde fila 2 (fila 1 es encabezados)
                for (int row = 2; row <= rowCount; row++)
                {
                    var productoIdCell = worksheet.Cell(row, 1).GetValue<string>();
                    var nombrePresentacionCell = worksheet.Cell(row, 2).GetValue<string>();
                    var valorOpcionCell = worksheet.Cell(row, 3).GetValue<string>();

                    // Validar campos obligatorios
                    if (string.IsNullOrWhiteSpace(productoIdCell) || 
                        string.IsNullOrWhiteSpace(nombrePresentacionCell) || 
                        string.IsNullOrWhiteSpace(valorOpcionCell))
                    {
                        continue; // Saltar filas incompletas
                    }

                    bool productoIdParsed = int.TryParse(productoIdCell, out int productoId);
                    if (!productoIdParsed)
                        continue;

                    bool precioParsed = decimal.TryParse(worksheet.Cell(row, 4).GetValue<string>(), out decimal precioOpcion);
                    bool stockParsed = int.TryParse(worksheet.Cell(row, 6).GetValue<string>(), out int stockOpcion);

                    var fila = new PresentacionExcelRowDto
                    {
                        ProductoID = productoId,
                        NombrePresentacion = nombrePresentacionCell,
                        ValorOpcion = valorOpcionCell,
                        PrecioOpcion = precioParsed ? precioOpcion : null,
                        ImagenOpcion = worksheet.Cell(row, 5).GetValue<string>(),
                        StockOpcion = stockParsed ? stockOpcion : null,
                        DescripcionOpcion = worksheet.Cell(row, 7).GetValue<string>()
                    };

                    filas.Add(fila);
                }
            }

            return filas;
        }
    }
}
