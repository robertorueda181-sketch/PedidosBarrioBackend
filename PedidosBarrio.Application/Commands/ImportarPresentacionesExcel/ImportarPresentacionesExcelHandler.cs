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
                                Precios = precios.Select(p => new PrecioCreateDto
                                {
                                    PrecioValor = p.Value.Valor,
                                    Descripcion = p.Key,
                                    EsPrincipal = p.Value.Principal == true
                                }).ToList(),
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
                                Precios = precios.Select(p => new PrecioDto
                                {
                                    IdPrecio = 0,
                                    PrecioValor = p.Value.Valor,
                                    EsPrincipal = p.Value.Principal == true,
                                    Descripcion = p.Key
                                }).ToList()
                            };

                            await _mediator.Send(new UpdateProductoCommand(productoId, updateDto));
                            result.ProductosActualizados++;
                        }

                        result.ProductosProcesados.Add(productoId);

                        // Procesar presentaciones y opciones
                        var filasOpciones = grupoProducto
                            .Where(f => !string.IsNullOrWhiteSpace(f.NombrePresentacion) && !string.IsNullOrWhiteSpace(f.DescripcionOpcion))
                            .ToList();

                        if (filasOpciones.Any())
                        {
                            var presentacionesExistentes = (await _presentacionRepository.GetByProductoIdAsync(productoId)).ToList();
                            var presentacionesByNombre = presentacionesExistentes
                                .GroupBy(p => p.Descripcion.Trim(), StringComparer.OrdinalIgnoreCase)
                                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

                            foreach (var grupoPresentacion in filasOpciones.GroupBy(f => f.NombrePresentacion!.Trim(), StringComparer.OrdinalIgnoreCase))
                            {
                                var nombrePres = grupoPresentacion.Key.Trim();
                                if (!presentacionesByNombre.TryGetValue(nombrePres, out var presentacion))
                                {
                                    var nueva = new Presentacion(nombrePres, empresaId, productoId);
                                    var presentacionId = await _presentacionRepository.AddAsync(nueva);
                                    nueva.PresentacionID = presentacionId;
                                    presentacion = nueva;
                                    presentacionesByNombre[nombrePres] = presentacion;
                                    result.PresentacionesCreadas++;
                                }

                                var opcionesExistentes = await _presentacionOpcionRepository.GetByPresentacionIdAsync(presentacion.PresentacionID);
                                var opcionesByValor = opcionesExistentes
                                    .Where(o => !string.IsNullOrWhiteSpace(o.Valor))
                                    .ToDictionary(o => o.Valor.Trim().ToLowerInvariant(), o => o);

                                foreach (var fila in grupoPresentacion)
                                {
                                    var valor = fila.DescripcionOpcion!.Trim();
                                    var valorKey = valor.ToLowerInvariant();
                                    var descripcion = string.IsNullOrWhiteSpace(fila.DescripcionOpcion) ? null : fila.DescripcionOpcion.Trim();

                                    if (opcionesByValor.TryGetValue(valorKey, out var existente))
                                    {
                                        if (fila.Precio.HasValue) existente.Precio = fila.Precio;
                                        if (descripcion != null) existente.Descripcion = descripcion;
                                        existente.Activa = true;
                                        await _presentacionOpcionRepository.UpdateAsync(existente);
                                        result.OpcionesActualizadas++;
                                    }
                                    else
                                    {
                                        var nuevaOpcion = new PresentacionOpcion(valor, presentacion.PresentacionID, fila.Precio, "")
                                        {
                                            Stock = 0,
                                            Descripcion = descripcion,
                                            Activa = true
                                        };
                                        await _presentacionOpcionRepository.AddAsync(nuevaOpcion);
                                        opcionesByValor[valorKey] = nuevaOpcion;
                                        result.OpcionesAgregadas++;
                                    }
                                }
                            }
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
