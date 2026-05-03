using ClosedXML.Excel;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PedidosBarrio.Application.Services
{
    public interface IPresentacionExcelService
    {
        Task<byte[]> GenerarPlantillaAsync(Guid empresaId, ICategoriaRepository categoriaRepository);
        Task<byte[]> GenerarPlantillaVaciaAsync(Guid empresaId, ICategoriaRepository categoriaRepository);
        Task<List<PresentacionExcelRowDto>> LeerPresentacionesAsync(Stream excelStream);
    }

    public class PresentacionExcelService : IPresentacionExcelService
    {
        public async Task<byte[]> GenerarPlantillaAsync(Guid empresaId, ICategoriaRepository categoriaRepository)
        {
            using var workbook = new XLWorkbook();

            // Hoja Catalogo
            var ws = workbook.Worksheets.Add("Catalogo");
            string[] headers = {
                "Codigo", "Categoria*", "NombreProducto*", "DescripcionProducto",
                "Stock", "StockMinimo", "Inventario", "Visible",
                "PrecioValor", "PrecioDescripcion", "PrecioEsPrincipal",
                "NombrePresentacion", "ValorOpcion", "PrecioOpcion",
                "ImagenOpcion", "StockOpcion", "DescripcionOpcion"
            };
            for (int i = 0; i < headers.Length; i++)
                ws.Cell(1, i + 1).Value = headers[i];

            var hdr = ws.Row(1);
            hdr.Style.Font.Bold = true;
            hdr.Style.Fill.BackgroundColor = XLColor.LightBlue;
            hdr.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            int[] widths = { 12, 12, 28, 35, 10, 12, 10, 10, 12, 18, 16, 20, 16, 12, 30, 12, 24 };
            for (int i = 0; i < widths.Length; i++)
                ws.Column(i + 1).Width = widths[i];

            // Ejemplos en fila 2
            ws.Cell(2, 1).Value = "PRD001"; ws.Cell(2, 2).Value = "Bebidas"; ws.Cell(2, 3).Value = "Polo Básico";
            ws.Cell(2, 4).Value = "Polo de algodón"; ws.Cell(2, 5).Value = 100; ws.Cell(2, 6).Value = 5;
            ws.Cell(2, 7).Value = true; ws.Cell(2, 8).Value = true; ws.Cell(2, 9).Value = 25.90m;
            ws.Cell(2, 10).Value = "General"; ws.Cell(2, 11).Value = true; ws.Cell(2, 12).Value = "Talla";
            ws.Cell(2, 13).Value = "S"; ws.Cell(2, 14).Value = ""; ws.Cell(2, 15).Value = "";
            ws.Cell(2, 16).Value = 50; ws.Cell(2, 17).Value = "Talla Pequeña";

            // Hoja Categoria
            var catWs = workbook.Worksheets.Add("Categoria");
            catWs.Cell(1, 1).Value = "CategoriaID";
            catWs.Cell(1, 2).Value = "Nombre*";
            catWs.Cell(1, 3).Value = "Color";
            catWs.Cell(1, 4).Value = "Activa";
            var catHdr = catWs.Row(1);
            catHdr.Style.Font.Bold = true;
            catHdr.Style.Fill.BackgroundColor = XLColor.LightGreen;
            catHdr.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Obtener categorías reales
            var categorias = (await categoriaRepository.GetByEmpresaIdAsync(empresaId))
                .OrderBy(c => c.CategoriaID).ToList();

            int row = 2;
            foreach (var c in categorias)
            {
                catWs.Cell(row, 1).Value = c.CategoriaID;
                catWs.Cell(row, 2).Value = c.Descripcion;
                catWs.Cell(row, 3).Value = c.Color ?? "";
                catWs.Cell(row, 4).Value = c.Activa ?? true;
                row++;
            }

            int[] catWidths = { 15, 30, 20, 10 };
            for (int i = 0; i < catWidths.Length; i++)
                catWs.Column(i + 1).Width = catWidths[i];

            // Hoja Instrucciones
            var noteWs = workbook.Worksheets.Add("Instrucciones");
            noteWs.Cell(1, 1).Value = "Instrucciones:";
            noteWs.Cell(2, 1).Value = "1) Hoja 'Categoria': Lista las categorías de tu empresa. Agrega o modifica filas.";
            noteWs.Cell(3, 1).Value = "2) Hoja 'Catalogo': Importa productos, precios y presentaciones.";
            noteWs.Cell(4, 1).Value = "   - ProductoID vacío = nuevo producto. Con ID = actualizar (merge).";
            noteWs.Cell(5, 1).Value = "   - *CategoriaID y NombreProducto son obligatorios.";
            noteWs.Cell(6, 1).Value = "   - Precios: 'PrecioValor' obligatorio. 'PrecioDescripcion' opcional (def: General).";
            noteWs.Cell(7, 1).Value = "   - Presentaciones: 'NombrePresentacion' y 'ValorOpcion'. Múltiples filas por producto.";
            noteWs.Column(1).Width = 80;

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }

        public async Task<byte[]> GenerarPlantillaVaciaAsync(Guid empresaId, ICategoriaRepository categoriaRepository)
        {
            using var workbook = new XLWorkbook();

            // Hoja Catalogo (vacía con solo encabezados)
            var ws = workbook.Worksheets.Add("Catalogo");
            string[] headers = {
                "Codigo", "Categoria*", "NombreProducto*", "DescripcionProducto",
                "Stock", "StockMinimo", "Inventario", "Visible",
                "PrecioValor", "PrecioDescripcion", "PrecioEsPrincipal",
                "NombrePresentacion", "ValorOpcion", "PrecioOpcion",
                "ImagenOpcion", "StockOpcion", "DescripcionOpcion"
            };
            for (int i = 0; i < headers.Length; i++)
                ws.Cell(1, i + 1).Value = headers[i];

            var hdr = ws.Row(1);
            hdr.Style.Font.Bold = true;
            hdr.Style.Fill.BackgroundColor = XLColor.LightBlue;
            hdr.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            int[] widths = { 12, 12, 28, 35, 10, 12, 10, 10, 12, 18, 16, 20, 16, 12, 30, 12, 24 };
            for (int i = 0; i < widths.Length; i++)
                ws.Column(i + 1).Width = widths[i];

            // Hoja Categoria
            var catWs = workbook.Worksheets.Add("Categoria");
            catWs.Cell(1, 1).Value = "CategoriaID";
            catWs.Cell(1, 2).Value = "Nombre*";
            catWs.Cell(1, 3).Value = "Color";
            catWs.Cell(1, 4).Value = "Activa";
            var catHdr = catWs.Row(1);
            catHdr.Style.Font.Bold = true;
            catHdr.Style.Fill.BackgroundColor = XLColor.LightGreen;
            catHdr.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Obtener categorías reales
            var categorias = (await categoriaRepository.GetByEmpresaIdAsync(empresaId))
                .OrderBy(c => c.CategoriaID).ToList();

            int row = 2;
            foreach (var c in categorias)
            {
                catWs.Cell(row, 1).Value = c.CategoriaID;
                catWs.Cell(row, 2).Value = c.Descripcion;
                catWs.Cell(row, 3).Value = c.Color ?? "";
                catWs.Cell(row, 4).Value = c.Activa ?? true;
                row++;
            }

            int[] catWidths = { 15, 30, 20, 10 };
            for (int i = 0; i < catWidths.Length; i++)
                catWs.Column(i + 1).Width = catWidths[i];

            // Hoja Instrucciones
            var noteWs = workbook.Worksheets.Add("Instrucciones");
            noteWs.Cell(1, 1).Value = "Instrucciones:";
            noteWs.Cell(2, 1).Value = "1) Hoja 'Categoria': Lista las categorías de tu empresa. Agrega o modifica filas.";
            noteWs.Cell(3, 1).Value = "2) Hoja 'Catalogo': Importa productos, precios y presentaciones.";
            noteWs.Cell(4, 1).Value = "   - ProductoID vacío = nuevo producto. Con ID = actualizar (merge).";
            noteWs.Cell(5, 1).Value = "   - *CategoriaID y NombreProducto son obligatorios.";
            noteWs.Cell(6, 1).Value = "   - Precios: 'PrecioValor' obligatorio. 'PrecioDescripcion' opcional (def: General).";
            noteWs.Cell(7, 1).Value = "   - Presentaciones: 'NombrePresentacion' y 'ValorOpcion'. Múltiples filas por producto.";
            noteWs.Column(1).Width = 80;

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }

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
                    NombrePresentacion = ws.Cell(row, 8).GetValue<string>(),
                    DescripcionOpcion = ws.Cell(row, 9).GetValue<string>()
                });
            }

            return result;
        }
    }
}
