using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.CargaMasivaImagenes
{
    public class CargaMasivaImagenesCommandHandler : IRequestHandler<CargaMasivaImagenesCommand, CargaMasivaImagenesResponseDto>
    {
        private readonly IImageProcessingService _imageProcessingService;
        private readonly IPresentacionOpcionRepository _presentacionOpcionRepository;
        private readonly IPresentacionRepository _presentacionRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CargaMasivaImagenesCommandHandler> _logger;

        public CargaMasivaImagenesCommandHandler(
            IImageProcessingService imageProcessingService,
            IPresentacionOpcionRepository presentacionOpcionRepository,
            IPresentacionRepository presentacionRepository,
            IProductoRepository productoRepository,
            IConfiguration configuration,
            ILogger<CargaMasivaImagenesCommandHandler> logger)
        {
            _imageProcessingService = imageProcessingService;
            _presentacionOpcionRepository = presentacionOpcionRepository;
            _presentacionRepository = presentacionRepository;
            _productoRepository = productoRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<CargaMasivaImagenesResponseDto> Handle(CargaMasivaImagenesCommand request, CancellationToken cancellationToken)
        {
            var response = new CargaMasivaImagenesResponseDto();

            if (request.Imagenes == null || !request.Imagenes.Any())
            {
                response.Errores.Add("Debe proporcionar al menos un archivo de imagen");
                return response;
            }

            foreach (var file in request.Imagenes)
            {
                if (file.Length == 0)
                {
                    response.Errores.Add($"{file.FileName}: Archivo vacío");
                    continue;
                }

                try
                {
                    var fileName = Path.GetFileNameWithoutExtension(file.FileName);

                    // Validar tamaño (máximo 10MB)
                    if (file.Length > 10 * 1024 * 1024)
                    {
                        response.Errores.Add($"{file.FileName}: Excede el tamaño máximo de 10MB");
                        continue;
                    }

                    // Buscar el producto por código
                    var codigoProducto = fileName;
                    var partesNombre = new List<string>();
                    var productos = await _productoRepository.GetByEmpresaIdAsync(request.EmpresaId);
                    var producto = productos.FirstOrDefault(p =>
                        p.Codigo != null &&
                        p.Codigo.Equals(codigoProducto, StringComparison.OrdinalIgnoreCase));

                    if (producto == null && fileName.Contains('-'))
                    {
                        var posibleCodigo = fileName.Split('-')[0];
                        producto = productos.FirstOrDefault(p =>
                            p.Codigo != null &&
                            p.Codigo.Equals(posibleCodigo, StringComparison.OrdinalIgnoreCase));
                        if (producto != null)
                        {
                            codigoProducto = posibleCodigo;
                            partesNombre = fileName.Split('-').Skip(1).ToList();
                        }
                    }

                    if (producto == null)
                    {
                        response.Errores.Add($"{file.FileName}: Producto con código '{codigoProducto}' no encontrado");
                        continue;
                    }

                    var presentaciones = (await _presentacionRepository.GetByProductoIdAsync(producto.ProductoID)).ToList();
                    var descripcionOpcion = partesNombre.Any() ? string.Join("/", partesNombre) : "";
                    PresentacionOpcion? opcionEncontrada = null;

                    if (!partesNombre.Any())
                    {
                        foreach (var pres in presentaciones)
                        {
                            var opciones = await _presentacionOpcionRepository.GetByPresentacionIdAsync(pres.PresentacionID);
                            opcionEncontrada = opciones!.FirstOrDefault(o =>
                                (string.IsNullOrWhiteSpace(o.Descripcion)) ||
                                (o.Descripcion?.Equals("General", StringComparison.OrdinalIgnoreCase) == true) ||
                                (o.Valor?.Equals("General", StringComparison.OrdinalIgnoreCase) == true));
                            if (opcionEncontrada != null) break;

                            opcionEncontrada = opciones!.FirstOrDefault();
                            if (opcionEncontrada != null) break;
                        }
                    }
                    else
                    {
                        foreach (var pres in presentaciones)
                        {
                            var opciones = await _presentacionOpcionRepository.GetByPresentacionIdAsync(pres.PresentacionID);
                            opcionEncontrada = opciones!.FirstOrDefault(o =>
                                (o.Descripcion != null && o.Descripcion.Equals(descripcionOpcion, StringComparison.OrdinalIgnoreCase)) ||
                                (o.Valor != null && o.Valor.Equals(string.Join("/", partesNombre), StringComparison.OrdinalIgnoreCase)) ||
                                partesNombre.Any(p => (o.Valor != null && o.Valor.Contains(p, StringComparison.OrdinalIgnoreCase)) ||
                                                      (o.Descripcion != null && o.Descripcion.Contains(p, StringComparison.OrdinalIgnoreCase))));
                            if (opcionEncontrada != null) break;
                        }

                        if (opcionEncontrada == null)
                        {
                            foreach (var pres in presentaciones)
                            {
                                var opciones = await _presentacionOpcionRepository.GetByPresentacionIdAsync(pres.PresentacionID);
                                foreach (var variante in partesNombre)
                                {
                                    opcionEncontrada = opciones!.FirstOrDefault(o =>
                                        (o.Valor != null && o.Valor.Contains(variante, StringComparison.OrdinalIgnoreCase)) ||
                                        (o.Descripcion != null && o.Descripcion.Contains(variante, StringComparison.OrdinalIgnoreCase)));
                                    if (opcionEncontrada != null) break;
                                }
                                if (opcionEncontrada != null) break;
                            }
                        }
                    }

                    if (opcionEncontrada == null)
                    {
                        foreach (var pres in presentaciones)
                        {
                            var opciones = await _presentacionOpcionRepository.GetByPresentacionIdAsync(pres.PresentacionID);
                            foreach (var variante in partesNombre)
                            {
                                opcionEncontrada = opciones!.FirstOrDefault(o =>
                                    (o.Valor?.Contains(variante, StringComparison.OrdinalIgnoreCase) == true) ||
                                    (o.Descripcion?.Contains(variante, StringComparison.OrdinalIgnoreCase) == true));
                                if (opcionEncontrada != null) break;
                            }
                            if (opcionEncontrada != null) break;
                        }
                    }

                    if (opcionEncontrada == null)
                    {
                        response.Errores.Add($"{file.FileName}: No se encontró opción para '{descripcionOpcion}'");
                        continue;
                    }

                    var urlImagen = await _imageProcessingService.OptimizeAndSaveImageAsync(
                        file.Stream, producto.ProductoID, request.EmpresaId);

                    opcionEncontrada!.Imagen = _configuration["BaseUrl"] + urlImagen;
                    await _presentacionOpcionRepository.UpdateAsync(opcionEncontrada);

                    response.Exitosas.Add(new ImagenCargadaDto
                    {
                        File = file.FileName,
                        ProductoCodigo = codigoProducto,
                        ProductoId = producto.ProductoID,
                        OpcionDescripcion = descripcionOpcion,
                        OpcionId = opcionEncontrada.PresentacionOpcionID,
                        ImagenUrl = urlImagen,
                        Estado = "OK"
                    });
                }
                catch (Exception ex)
                {
                    response.Errores.Add($"{file.FileName}: {ex.Message}");
                    _logger.LogError(ex, $"Error procesando imagen {file.FileName}");
                }
            }

            response.Mensaje = $"Carga masiva completada: {response.Exitosas.Count} exitosas, {response.Errores.Count} errores";
            
            return response;
        }
    }
}
