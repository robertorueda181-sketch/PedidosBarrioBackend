using System.Collections.Concurrent;

namespace PedidosBarrio.Application.Services;

/// <summary>
/// DTO para enviar eventos de PageView a través de la cola
/// </summary>
public record PageViewEvent(
    Guid EmpresaID,
    string Url,
    DateTime Fecha,
    string? UserAgent = null,
    string? IpAddress = null,
    string? Referrer = null
);

/// <summary>
/// Interfaz para el servicio de cola de PageViews
/// </summary>
public interface IPageViewQueueService
{
    /// <summary>
    /// Encoloca un evento de PageView para procesamiento asíncrono
    /// </summary>
    Task EnqueuePageViewAsync(PageViewEvent pageViewEvent);

    /// <summary>
    /// Obtiene el siguiente evento de la cola
    /// </summary>
    Task<PageViewEvent?> DequeuePageViewAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el tamaño actual de la cola
    /// </summary>
    int GetQueueSize();

    /// <summary>
    /// Obtiene estadísticas de la cola
    /// </summary>
    QueueStats GetQueueStats();

    /// <summary>
    /// Resetea las estadísticas de la cola
    /// </summary>
    void ResetStats();
}

/// <summary>
/// Estadísticas de la cola de PageViews
/// </summary>
public record QueueStats(
    int CurrentQueueSize,
    long TotalEnqueued,
    long TotalDequeued,
    long TotalDiscarded,
    DateTime LastEnqueueTime,
    DateTime LastDequeueTime
);

/// <summary>
/// Implementación de la cola de PageViews en memoria usando BlockingCollection
/// Rápido, eficiente y no requiere dependencias externas
/// </summary>
public class PageViewQueueService : IPageViewQueueService
{
    private readonly BlockingCollection<PageViewEvent> _queue;
    private const int MaxQueueSize = 10000; // Máximo de eventos en cola antes de procesar
    private long _totalEnqueued = 0;
    private long _totalDequeued = 0;
    private long _totalDiscarded = 0;
    private DateTime _lastEnqueueTime = DateTime.UtcNow;
    private DateTime _lastDequeueTime = DateTime.UtcNow;
    private readonly object _statsLock = new object();

    public PageViewQueueService()
    {
        // BoundedCapacity previene que la cola crezca infinitamente
        _queue = new BlockingCollection<PageViewEvent>(MaxQueueSize);
    }

    public async Task EnqueuePageViewAsync(PageViewEvent pageViewEvent)
    {
        try
        {
            // Intenta agregar el evento a la cola con timeout (5000ms = 5 segundos)
            if (!_queue.TryAdd(pageViewEvent, 5000))
            {
                lock (_statsLock)
                {
                    _totalDiscarded++;
                }
                // Si la cola está llena, descartamos el evento
                throw new InvalidOperationException("PageView queue is full. Event discarded.");
            }

            lock (_statsLock)
            {
                _totalEnqueued++;
                _lastEnqueueTime = DateTime.UtcNow;
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error enqueuing PageView event: {ex.Message}", ex);
        }
    }

    public async Task<PageViewEvent?> DequeuePageViewAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Intenta obtener un evento con timeout (1000ms = 1 segundo)
            if (_queue.TryTake(out var pageViewEvent, 1000, cancellationToken))
            {
                lock (_statsLock)
                {
                    _totalDequeued++;
                    _lastDequeueTime = DateTime.UtcNow;
                }
                return pageViewEvent;
            }

            return null;
        }
        catch (OperationCanceledException)
        {
            return null;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error dequeuing PageView event: {ex.Message}", ex);
        }
    }

    public int GetQueueSize()
    {
        return _queue.Count;
    }

    public QueueStats GetQueueStats()
    {
        lock (_statsLock)
        {
            return new QueueStats(
                CurrentQueueSize: _queue.Count,
                TotalEnqueued: _totalEnqueued,
                TotalDequeued: _totalDequeued,
                TotalDiscarded: _totalDiscarded,
                LastEnqueueTime: _lastEnqueueTime,
                LastDequeueTime: _lastDequeueTime
            );
        }
    }

    public void ResetStats()
    {
        lock (_statsLock)
        {
            _totalEnqueued = 0;
            _totalDequeued = 0;
            _totalDiscarded = 0;
            _lastEnqueueTime = DateTime.UtcNow;
            _lastDequeueTime = DateTime.UtcNow;
        }
    }
}
