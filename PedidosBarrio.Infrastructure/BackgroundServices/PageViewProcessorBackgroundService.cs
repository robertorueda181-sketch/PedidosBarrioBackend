using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Infrastructure.BackgroundServices;

/// <summary>
/// Worker background que procesa continuamente la cola de PageViews
/// Agrupa los eventos en lotes y los guarda en BD sin bloquear las peticiones HTTP
/// </summary>
public class PageViewProcessorBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PageViewProcessorBackgroundService> _logger;
    private const int BatchSize = 100; // Procesar de 100 en 100
    private const int DelayMs = 5000; // Procesar cada 5 segundos o cuando se complete un lote
    private int _totalProcessed = 0;
    private int _totalErrors = 0;

    public PageViewProcessorBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<PageViewProcessorBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🚀 PageView Processor Background Service iniciado");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPageViewsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al procesar PageViews");
                _totalErrors++;
            }

            // Esperar antes de la siguiente iteración
            await Task.Delay(DelayMs, stoppingToken);
        }

        _logger.LogInformation($"⏹️ PageView Processor Background Service detenido. Total procesados: {_totalProcessed}, Errores: {_totalErrors}");
    }

    private async Task ProcessPageViewsAsync(CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var pageViewQueueService = scope.ServiceProvider.GetRequiredService<IPageViewQueueService>();
            var pageViewRepository = scope.ServiceProvider.GetRequiredService<IPageViewRepository>();

            var batch = new List<PageView>();
            PageViewEvent? pageViewEvent;

            // Recopilar un lote de eventos de la cola
            while (batch.Count < BatchSize && 
                   (pageViewEvent = await pageViewQueueService.DequeuePageViewAsync(cancellationToken)) != null)
            {
                var pageView = new PageView
                {
                    EmpresaID = pageViewEvent.EmpresaID,
                    Url = pageViewEvent.Url,
                    Fecha = pageViewEvent.Fecha,
                    UserAgent = pageViewEvent.UserAgent,
                    IpAddress = pageViewEvent.IpAddress,
                    Referrer = pageViewEvent.Referrer,
                    Processed = false,
                    CreatedAt = DateTime.UtcNow
                };

                batch.Add(pageView);
            }

            // Si hay eventos en el lote, guardarlos en BD
            if (batch.Count > 0)
            {
                try
                {
                    var result = await pageViewRepository.AddBulkAsync(batch);
                    _totalProcessed += batch.Count;
                    _logger.LogInformation($"✅ PageViews procesados: {batch.Count} registros guardados | Total: {_totalProcessed}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"❌ Error al guardar {batch.Count} PageViews en BD");
                    _totalErrors++;
                }
            }
        }
    }
}
