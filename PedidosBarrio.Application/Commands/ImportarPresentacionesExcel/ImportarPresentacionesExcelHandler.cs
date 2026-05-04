using DocumentFormat.OpenXml.Presentation;
using MediatR;
using PedidosBarrio.Application.Commands.CreateProducto;
using PedidosBarrio.Application.Commands.UpdateProducto;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using System.Linq;

namespace PedidosBarrio.Application.Commands.ImportarPresentacionesExcel
{
    public class ImportarPresentacionesExcelCommandHandler : IRequestHandler<ImportarPresentacionesExcelCommand, ImportarPresentacionesExcelResult>
    {
        private readonly IPresentacionExcelService _excelService;
        private readonly IProductoRepository _productoRepository;
        private readonly IPresentacionRepository _presentacionRepository;
        private readonly IPresentacionOpcionRepository _presentacionOpcionRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;
        private readonly IApplicationLogger _logger;

        public ImportarPresentacionesExcelCommandHandler(
            IPresentacionExcelService excelService,
            IProductoRepository productoRepository,
            IPresentacionRepository presentacionRepository,
            IPresentacionOpcionRepository presentacionOpcionRepository,
            ICategoriaRepository categoriaRepository,
            ICurrentUserService currentUserService,
            IMediator mediator,
            IApplicationLogger logger)
        {
            _excelService = excelService;
            _productoRepository = productoRepository;
            _presentacionRepository = presentacionRepository;
            _presentacionOpcionRepository = presentacionOpcionRepository;
            _categoriaRepository = categoriaRepository;
            _currentUserService = currentUserService;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<ImportarPresentacionesExcelResult> Handle(ImportarPresentacionesExcelCommand request, CancellationToken cancellationToken)
        {
            var result = new ImportarPresentacionesExcelResult();

            try
            {
                await _logger.LogInformationAsync($"Iniciando importación de Excel: {request.FileName}", "ImportarPresentacionesExcelCommand");

                // Validar archivo
                if (!request.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    var errorMsg = "Solo se aceptan archivos .xlsx";
                    result.Errores.Add(errorMsg);
                    await _logger.LogWarningAsync(errorMsg, "ImportarPresentacionesExcelCommand");
                    return result;
                }

                // Leer Excel (productos y presentaciones)
                var filas = await _excelService.LeerPresentacionesAsync(request.ExcelStream);

                if (!filas.Any())
                {
                    var errorMsg = "El archivo no contiene datos válidos";
                    result.Errores.Add(errorMsg);
                    await _logger.LogWarningAsync(errorMsg, "ImportarPresentacionesExcelCommand");
                    return result;
                }

                var empresaId = _currentUserService.GetEmpresaId();
                await _logger.LogInformationAsync($"Archivo leído: {filas.Count} filas. Empresa ID: {empresaId}", "ImportarPresentacionesExcelCommand");

                // Obtener categorías existentes de la empresa (para mapeo por descripción)
                var categoriasEmpresa = (await _categoriaRepository.GetByEmpresaIdAsync(empresaId)).ToList();
                var categoriasByDescripcion = categoriasEmpresa
                    .Where(c => !string.IsNullOrWhiteSpace(c.Descripcion))
                    .GroupBy(c => c.Descripcion.Trim().ToLowerInvariant())
                    .ToDictionary(g => g.Key, g => g.First());

                // Obtener productos existentes de la empresa (después de actualizar categorías)
                var productosEmpresa = (await _productoRepository.GetByEmpresaIdAsync(empresaId)).ToList();
                var productosByCodigo = productosEmpresa
                    .Where(p => !string.IsNullOrWhiteSpace(p.Codigo))
                    .GroupBy(p => p.Codigo.Trim(), StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

                // Filtrar filas con datos válidos
                var filasConDatos = filas
                    .Where(f => !string.IsNullOrWhiteSpace(f.Codigo) && !string.IsNullOrWhiteSpace(f.Categoria) && !string.IsNullOrWhiteSpace(f.NombreProducto))
                    .ToList();

                if (!filasConDatos.Any())
                {
                    var errorMsg = "No se encontraron filas con Codigo, Categoria y NombreProducto";
                    result.Errores.Add(errorMsg);
                    await _logger.LogWarningAsync(errorMsg, "ImportarPresentacionesExcelCommand");
                    return result;
                }

                // Agrupar por producto (por código)
                var grupos = filasConDatos
                    .GroupBy(f => f.Codigo.Trim(), StringComparer.OrdinalIgnoreCase)
                    .ToList();

                foreach (var grupoProducto in grupos)
                {
                    try
                    {
                        var first = grupoProducto.First();
                        if (string.IsNullOrWhiteSpace(first.Categoria) || string.IsNullOrWhiteSpace(first.NombreProducto))
                        {
                            var errorMsg = $"Fila {first.ExcelRow}: Categoria y NombreProducto son obligatorios";
                            result.Errores.Add(errorMsg);
                            await _logger.LogWarningAsync(errorMsg, "ImportarPresentacionesExcelCommand");
                            continue;
                        }

                        // Resolver CategoriaID por descripción
                        var categoriaKey = first.Categoria.Trim().ToLowerInvariant();
                        if (!categoriasByDescripcion.TryGetValue(categoriaKey, out var categoriaExistente))
                        {
                            var errorMsg = $"Fila {first.ExcelRow}: Categoría '{first.Categoria}' no encontrada. Asegúrese de que existe en la hoja 'Categoria' y que coincide exactamente.";
                            result.Errores.Add(errorMsg);
                            await _logger.LogWarningAsync(errorMsg, "ImportarPresentacionesExcelCommand");
                            continue;
                        }
                        var categoriaId = categoriaExistente.CategoriaID;

                        var codigoProducto = first.Codigo.Trim();

                        Producto? productoExistente = productosByCodigo.GetValueOrDefault(codigoProducto, null);

                        // Obtener datos del producto desde el grupo
                        var nombreProducto = first.NombreProducto.Trim();
                        var descripcionProducto = grupoProducto
                            .Select(f => f.DescripcionProducto)
                            .LastOrDefault(v => !string.IsNullOrWhiteSpace(v))
                            ?? productoExistente?.Descripcion
                            ?? string.Empty;


                        int stockMinimo = grupoProducto.Select(f => f.StockMinimo).LastOrDefault(v => v.HasValue)
                            ?? productoExistente?.StockMinimo
                            ?? 0;


                        var visible = grupoProducto.Select(f => f.Visible).LastOrDefault(v => v.HasValue)
                            ?? (productoExistente?.Visible ?? true);

                        // Procesar precios
                        var precios = new Dictionary<string, (decimal Valor, bool? Principal)>(StringComparer.OrdinalIgnoreCase);
                        foreach (var fila in grupoProducto)
                        {
                            if (!fila.Precio.HasValue) continue;
                            var desc = "General";
                            precios[desc] = (fila.Precio.Value, true);
                        }

                        if (precios.Count == 0)
                        {
                            precios["General"] = (0m, true);
                        }

                        if (precios.Count == 1)
                        {
                            var item = precios.First();
                            precios[item.Key] = (item.Value.Valor, true);
                        }
                        else
                        {
                            string? principalKey = precios.FirstOrDefault(p => p.Value.Principal == true).Key;
                            principalKey ??= precios.Keys.First();

                            var keys = precios.Keys.ToList();
                            foreach (var k in keys)
                            {
                                precios[k] = (precios[k].Valor, string.Equals(k, principalKey, StringComparison.OrdinalIgnoreCase));
                            }
                        }

                        int productoId;
                        if (productoExistente == null)
                        {
                            var createDto = new CreateProductoDto
                            {
                                Codigo = codigoProducto,
                                CategoriaDescripcion = first.Categoria.Trim(),
                                Nombre = nombreProducto,
                                Descripcion = descripcionProducto,
                                Stock = 0,
                                StockMinimo = stockMinimo,
                                Inventario = false,
                                ImagenUrl = string.Empty,
                                ImagenDescripcion = string.Empty
                            };

                            var resultProducto = await _mediator.Send(new CreateProductoCommand(createDto));
                            productoId = resultProducto.ProductoID;
                            var nuevoProducto = await _productoRepository.GetByIdAsync(productoId, empresaId);
                            if (nuevoProducto != null)
                                productosByCodigo[codigoProducto] = nuevoProducto;
                            result.ProductosCreados++;
                        }
                        else
                        {
                            productoId = productoExistente.ProductoID;
                            var updateDto = new UpdateProductoDto
                            {
                                Codigo = codigoProducto,
                                CategoriaDescripcion = first.Categoria.Trim(),
                                Nombre = nombreProducto,
                                Descripcion = descripcionProducto,
                                Stock = 0,
                                StockMinimo = stockMinimo,
                                Inventario = false,
                                Visible = visible is bool b ? b : true,
                            };

                            await _mediator.Send(new UpdateProductoCommand(productoId, updateDto));
                            result.ProductosActualizados++;
                        }

                        result.ProductosProcesados.Add(productoId);

                        // ===============================
                        // GENERAR OPCIONES CON PRECIO REAL (DESDE EXCEL)
                        // ===============================
                        // ===============================
                        // CREAR PRESENTACIONES
                        // ===============================
                        string primeraPresentacion = grupoProducto.FirstOrDefault()?.NombrePresentacion1?.Trim() ?? "";
                        string segundaPresentacion = grupoProducto.FirstOrDefault()?.NombrePresentacion2?.Trim() ?? "";
                        string terceraPresentacion = grupoProducto.FirstOrDefault()?.NombrePresentacion3?.Trim() ?? "";

                        bool tienePresentaciones =
                            !string.IsNullOrWhiteSpace(primeraPresentacion) ||
                            !string.IsNullOrWhiteSpace(segundaPresentacion) ||
                            !string.IsNullOrWhiteSpace(terceraPresentacion);

                        // Si no tiene ninguna → usar GENERAL
                        if (!tienePresentaciones)
                        {
                            primeraPresentacion = "General";
                        }

                        int presentacionId1 = 0;

                        // Presentación principal
                        var nuevaPresentacion1 = new Presentacion(primeraPresentacion, empresaId, productoId);
                        presentacionId1 = await _presentacionRepository.AddAsync(nuevaPresentacion1);
                        result.PresentacionesCreadas++;

                        // Otras (solo metadata)
                        if (!string.IsNullOrWhiteSpace(segundaPresentacion))
                        {
                            await _presentacionRepository.AddAsync(new Presentacion(segundaPresentacion, empresaId, productoId));
                            result.PresentacionesCreadas++;
                        }

                        if (!string.IsNullOrWhiteSpace(terceraPresentacion))
                        {
                            await _presentacionRepository.AddAsync(new Presentacion(terceraPresentacion, empresaId, productoId));
                            result.PresentacionesCreadas++;
                        }


                        // ===============================
                        // GENERAR OPCIONES (SIN PERDER DUPLICADOS)
                        // ===============================

                        bool esPrimeraOpcion = true;

                        foreach (var fila in grupoProducto.Where(f => f.Precio.HasValue))
                        {
                            string op1 = fila.DescripcionOpcion1?.Trim() ?? "";
                            string op2 = fila.DescripcionOpcion2?.Trim() ?? "";
                            string op3 = fila.DescripcionOpcion3?.Trim() ?? "";

                            // Si no hay opciones → GENERAL
                            if (string.IsNullOrWhiteSpace(op1) &&
                                string.IsNullOrWhiteSpace(op2) &&
                                string.IsNullOrWhiteSpace(op3))
                            {
                                op1 = "General";
                            }

                            // Construir descripción
                            var partes = new List<string>();
                            if (!string.IsNullOrWhiteSpace(op1)) partes.Add(op1);
                            if (!string.IsNullOrWhiteSpace(op2)) partes.Add(op2);
                            if (!string.IsNullOrWhiteSpace(op3)) partes.Add(op3);

                            string descripcion = string.Join("/", partes);

                            decimal precio = fila.Precio ?? 0;

                            var nuevaOpcion = new PresentacionOpcion(op1, presentacionId1, precio, "")
                            {
                                Stock = 0,
                                Descripcion = descripcion,
                                Activa = true,
                                EsPrincipal = !tienePresentaciones
                                                ? true                 // caso GENERAL
                                                : esPrimeraOpcion      // solo la primera si hay presentaciones
                            };

                            await _presentacionOpcionRepository.AddAsync(nuevaOpcion);
                            result.OpcionesAgregadas++;

                            esPrimeraOpcion = false; // solo la primera será principal
                        }

                    }
                    catch (Exception ex)
                    {
                        var errorMsg = $"Error procesando grupo '{grupoProducto.Key}': {ex.Message}";
                        result.Errores.Add(errorMsg);
                        await _logger.LogErrorAsync(errorMsg, ex, "ImportarPresentacionesExcelCommand");
                    }
                }

                await _logger.LogInformationAsync(
                    $"Importación completada. Productos creados: {result.ProductosCreados}, actualizados: {result.ProductosActualizados}, presentaciones creadas: {result.PresentacionesCreadas}, opciones agregadas: {result.OpcionesAgregadas}, opciones actualizadas: {result.OpcionesActualizadas}",
                    "ImportarPresentacionesExcelCommand");

                return result;
            }
            catch (Exception ex)
            {
                var errorMsg = $"Error al procesar el archivo: {ex.Message}";
                result.Errores.Add(errorMsg);
                await _logger.LogErrorAsync(errorMsg, ex, "ImportarPresentacionesExcelCommand");
                return result;
            }
        }
    }
}
