using Microsoft.AspNetCore.Http;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Services;
using System.Diagnostics;
using System.Text.Json;

namespace PedidosBarrio.Infrastructure.Services
{
    public class DatabaseLogger : IApplicationLogger
    {
        private readonly ILogRepository _logRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserService _currentUserService;

        public DatabaseLogger(
            ILogRepository logRepository, 
            IHttpContextAccessor httpContextAccessor,
            ICurrentUserService currentUserService)
        {
            _logRepository = logRepository;
            _httpContextAccessor = httpContextAccessor;
            _currentUserService = currentUserService;
        }

        public async Task LogInformationAsync(string message, string category = "Information")
        {
            await LogAsync("Information", message, null, category);
        }

        public async Task LogWarningAsync(string message, string category = "Warning")
        {
            await LogAsync("Warning", message, null, category);
        }

        public async Task LogErrorAsync(string message, Exception exception = null, string category = "Error")
        {
            await LogAsync("Error", message, exception, category);
        }

        public async Task LogDebugAsync(string message, string category = "Debug")
        {
            await LogAsync("Debug", message, null, category);
        }

        private async Task LogAsync(string level, string message, Exception exception = null, string category = "General")
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;

                var logEntry = new LogEntry(level, "PedidosBarrio.Application", message, category)
                {
                    Exception = exception?.ToString(),
                    MachineName = Environment.MachineName,
                    ProcessId = Environment.ProcessId,
                    ThreadId = Environment.CurrentManagedThreadId
                };

                // Información de HTTP Context si está disponible
                if (httpContext != null)
                {
                    logEntry.RequestId = httpContext.TraceIdentifier;
                    logEntry.RequestPath = httpContext.Request.Path;
                    logEntry.HttpMethod = httpContext.Request.Method;

                    // Obtener información del usuario actual
                    try
                    {
                        var empresaId = _currentUserService.GetEmpresaId();
                        if (empresaId != Guid.Empty)
                        {
                            logEntry.EmpresaID = empresaId;
                        }

                        // Note: Using GetUserEmail() instead of GetUserId()
                        var userEmail = _currentUserService.GetUserEmail();
                        if (!string.IsNullOrEmpty(userEmail))
                        {
                            logEntry.UserId = userEmail;
                        }
                    }
                    catch
                    {
                        // Ignorar errores al obtener información del usuario
                        // (puede no estar autenticado)
                    }

                    // Propiedades adicionales como JSON
                    var properties = new
                    {
                        UserAgent = httpContext.Request.Headers.UserAgent.FirstOrDefault(),
                        RemoteIpAddress = httpContext.Connection.RemoteIpAddress?.ToString(),
                        Timestamp = DateTime.UtcNow,
                        Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
                    };

                    logEntry.Properties = JsonSerializer.Serialize(properties);
                }

                await _logRepository.AddLogAsync(logEntry);
            }
            catch (Exception ex)
            {
                // Si falla el logging a BD, escribir a archivo o consola como fallback
                Console.WriteLine($"Error logging to database: {ex.Message}");
                Console.WriteLine($"Original log: [{level}] {message}");
            }
        }
    }
}