using Microsoft.AspNetCore.Http;
using PedidosBarrio.Application.Logging;

namespace PedidosBarrio.Api.Services
{
    public interface IExcelSecurityService
    {
        /// <summary>
        /// Valida que un archivo Excel sea seguro para importar
        /// </summary>
        /// <returns>Una tupla con (esValido, mensajeError)</returns>
        Task<(bool Valid, string? ErrorMessage)> ValidarArchivoAsync(IFormFile archivo);

        /// <summary>
        /// Extrae el contenido del archivo de manera segura
        /// </summary>
        Task<Stream> ExtraerContenidoSeguroAsync(IFormFile archivo);
    }

    public class ExcelSecurityService : IExcelSecurityService
    {
        private const int MaximumFileSizeBytes = 5 * 1024 * 1024; // 5 MB
        private readonly string[] AllowedExtensions = { ".xlsx", ".xls" };
        private readonly string[] AllowedMimeTypes = 
        {
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "application/vnd.ms-excel"
        };

        /// <summary>
        /// Caracteres sospechosos que podrían indicar una inyección o script
        /// </summary>
        private readonly string[] SuspiciousPatterns = 
        {
            "VBA",
            "macro",
            "ActiveX",
            "http://",
            "https://",
            "ftp://",
            "javascript:",
            "PowerQuery",
            "WebQuery"
        };

        private readonly IApplicationLogger _logger;

        public ExcelSecurityService(IApplicationLogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<(bool Valid, string? ErrorMessage)> ValidarArchivoAsync(IFormFile archivo)
        {
            if (archivo == null)
                return (false, "El archivo es requerido");

            if (archivo.Length == 0)
                return (false, "El archivo está vacío");

            if (archivo.Length > MaximumFileSizeBytes)
                return (false, $"El archivo excede el tamaño máximo permitido ({MaximumFileSizeBytes / (1024 * 1024)} MB)");

            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                return (false, $"Extensión no permitida. Solo se aceptan: {string.Join(", ", AllowedExtensions)}");

            if (!AllowedMimeTypes.Contains(archivo.ContentType ?? string.Empty))
                return (false, "Tipo de contenido no válido. Se requiere un archivo Excel válido");

            // Validar nombre del archivo para prevenir path traversal
            var nombreArchivo = Path.GetFileName(archivo.FileName);
            if (nombreArchivo.Contains("..") || nombreArchivo.Contains("/") || nombreArchivo.Contains("\\"))
                return (false, "Nombre de archivo no válido");

            // Buscar patrones sospechosos en el nombre del archivo
            var nombreLower = nombreArchivo.ToLowerInvariant();
            foreach (var pattern in SuspiciousPatterns)
            {
                if (nombreLower.Contains(pattern.ToLowerInvariant()))
                    return (false, $"Nombre de archivo sospechoso (contiene: {pattern})");
            }

            // Validar contenido del archivo para detectar macros (análisis de bytes)
            try
            {
                var (contenidoValido, errorMensaje) = await ValidarContenidoExcelAsync(archivo);
                if (!contenidoValido)
                    return (false, errorMensaje);
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync($"Error validando contenido de Excel: {ex.Message}", ex, nameof(ExcelSecurityService));
                return (false, $"Error al validar contenido del archivo: {ex.Message}");
            }

            return (true, null);
        }

        private async Task<(bool Valid, string? ErrorMessage)> ValidarContenidoExcelAsync(IFormFile archivo)
        {
            try
            {
                using var stream = new MemoryStream();
                await archivo.CopyToAsync(stream);
                stream.Position = 0;

                var buffer = stream.ToArray();

                // Detectar firmas de archivos maliciosos comunes
                if (ContieneSignaturasOleDebugInfo(buffer))
                    return (false, "El archivo contiene propiedades sospechosas que podrían indicar malware");

                // Búsqueda de palabras clave de macros
                var contenidoTexto = System.Text.Encoding.UTF8.GetString(buffer, 0, Math.Min(buffer.Length, 10000));
                foreach (var pattern in SuspiciousPatterns)
                {
                    if (contenidoTexto.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                        return (false, $"El archivo contiene características sospechosas: {pattern}");
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                // Si no podemos validar, es mejor rechazar
                return (false, $"No se pudo validar el contenido del archivo: {ex.Message}");
            }
        }

        private bool ContieneSignaturasOleDebugInfo(byte[] buffer)
        {
            if (buffer.Length < 8)
                return false;

            // Firma OLE: D0 CF 11 E0
            if (buffer[0] == 0xD0 && buffer[1] == 0xCF && buffer[2] == 0x11 && buffer[3] == 0xE0)
            {
                // Es un archivo OLE (posible XLS viejo con macros)
                // Buscar patrones que indiquen macros en OLE
                var bufferStr = System.Text.Encoding.ASCII.GetString(buffer);
                return bufferStr.Contains("Macros") || bufferStr.Contains("_VBA_PROJECT");
            }

            return false;
        }

        public async Task<Stream> ExtraerContenidoSeguroAsync(IFormFile archivo)
        {
            var stream = new MemoryStream();
            await archivo.CopyToAsync(stream);
            stream.Position = 0;
            return stream;
        }
    }
}
