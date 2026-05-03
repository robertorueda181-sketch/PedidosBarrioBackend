using MediatR;
using PedidosBarrio.Application.Commands.ImportarPresentacionesExcel;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.ImportarProductosMasivos
{
    /// <summary>
    /// Handler para importar productos de forma masiva desde un archivo Excel con validaciones de seguridad
    /// </summary>
    public class ImportarProductosMasivosCommandHandler : IRequestHandler<ImportarProductosMasivosCommand, ImportarProductosMasivosResponseDto>
    {
        private readonly IPresentacionExcelService _excelService;
        private readonly IMediator _mediator;
        private readonly IApplicationLogger _logger;

        public ImportarProductosMasivosCommandHandler(
            IPresentacionExcelService excelService,
            IMediator mediator,
            IApplicationLogger logger)
        {
            _excelService = excelService ?? throw new ArgumentNullException(nameof(excelService));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ImportarProductosMasivosResponseDto> Handle(
            ImportarProductosMasivosCommand request,
            CancellationToken cancellationToken)
        {
            var response = new ImportarProductosMasivosResponseDto();

            try
            {
                await _logger.LogInformationAsync(
                    $"Iniciando importación masiva de productos. Archivo: {request.NombreArchivo}, Empresa: {request.EmpresaId}",
                    "ImportarProductosMasivosCommand");

                // El archivo ya fue validado en el endpoint, aquí solo lo procesamos
                await _logger.LogInformationAsync(
                    "Archivo pasó validaciones de seguridad",
                    "ImportarProductosMasivosCommand");

                // Delegar al comando existente de importación
                var commandImportacion = new ImportarPresentacionesExcelCommand(request.ArchivoStream, request.NombreArchivo);
                var resultadoImportacion = await _mediator.Send(commandImportacion, cancellationToken);

                // Mapear resultado
                response.ProductosCreados = resultadoImportacion.ProductosCreados;
                response.ProductosActualizados = resultadoImportacion.ProductosActualizados;
                response.PresentacionesCreadas = resultadoImportacion.PresentacionesCreadas;
                response.OpcionesAgregadas = resultadoImportacion.OpcionesAgregadas;
                response.OpcionesActualizadas = resultadoImportacion.OpcionesActualizadas;
                response.ProductosProcesados = resultadoImportacion.ProductosProcesados;
                response.Errores = resultadoImportacion.Errores;

                if (response.Exitoso)
                {
                    await _logger.LogInformationAsync(
                        $"Importación completada exitosamente. Productos creados: {response.ProductosCreados}, " +
                        $"Productos actualizados: {response.ProductosActualizados}, " +
                        $"Presentaciones creadas: {response.PresentacionesCreadas}",
                        "ImportarProductosMasivosCommand");
                }
                else
                {
                    await _logger.LogWarningAsync(
                        $"Importación completada con errores: {string.Join("; ", response.Errores)}",
                        "ImportarProductosMasivosCommand");
                }

                return response;
            }
            catch (Exception ex)
            {
                var msgError = $"Error crítico durante importación masiva: {ex.Message}";
                response.Errores.Add(msgError);
                await _logger.LogErrorAsync(msgError, ex, "ImportarProductosMasivosCommand");
                return response;
            }
        }
    }
}
