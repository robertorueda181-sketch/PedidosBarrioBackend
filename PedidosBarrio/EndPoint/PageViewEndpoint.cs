using MediatR;
using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.Commands.RegisterPageView;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Api.EndPoint;

public static class PageViewEndpoint
{
    public static void MapPageViewEndpoints(this IEndpointRouteBuilder app)
    {
        // Grupo PÚBLICO sin autenticación - para registrar visitas
        var publicGroup = app.MapGroup("/api/PageViews")
            .WithTags("Analytics");

        // POST /api/PageViews/track - Registrar una visita
        publicGroup.MapPost("/track", async (
            [FromBody] PageViewTrackRequest request,
            IMediator mediator,
            HttpContext httpContext) =>
        {
            // Obtener IP real (considerando proxies)
            var ipAddress = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                ?? httpContext.Connection.RemoteIpAddress?.ToString()
                ?? "unknown";

            // Obtener User-Agent
            var userAgent = httpContext.Request.Headers["User-Agent"].ToString();

            // Obtener Referer
            var referrer = httpContext.Request.Headers["Referer"].FirstOrDefault();

            var command = new RegisterPageViewCommand(
                request.CodigoEmpresa, // Usar codigoEmpresa en lugar de empresaId
                request.Url,
                userAgent,
                ipAddress,
                referrer
            );

            var result = await mediator.Send(command);

            // Siempre devolver 204 No Content - no esperamos respuesta
            return Results.NoContent();
        })
        .WithName("TrackPageView")
        .WithOpenApi()
        .WithSummary("📊 Registrar visita de página")
        .WithDescription("Registra una visita a una página de un negocio. No requiere autenticación. Retorna 204 No Content. El parámetro codigoEmpresa (ej: 'EMPRESA-001') se resuelve internamente a EmpresaID.")
        .Produces(StatusCodes.Status204NoContent);

        // GET /api/PageViews/stats/{empresaId} - Obtener estadísticas de visitas (solo con autenticación)
        var protectedGroup = app.MapGroup("/api/PageViews")
            .WithTags("Analytics")
            .RequireAuthorization();

        protectedGroup.MapGet("/stats/{empresaId:guid}", async (
            Guid empresaId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            IPageViewRepository pageViewRepository) =>
        {
            var start = startDate ?? DateTime.UtcNow.AddDays(-30);
            var end = endDate ?? DateTime.UtcNow;

            var pageViews = await pageViewRepository.GetByEmpresaAndDateRangeAsync(
                empresaId,
                start,
                end
            );

            var topUrls = await pageViewRepository.GetTopUrlsByEmpresaAsync(empresaId, 10);
            var totalCount = await pageViewRepository.GetCountByEmpresaAsync(empresaId);

            return Results.Ok(new
            {
                empresaId,
                totalViews = totalCount,
                viewsInRange = pageViews.Count(),
                startDate = start,
                endDate = end,
                topUrls = topUrls.Select(x => new { url = x.Url, count = x.Count }),
                pageViews = pageViews.Select(pv => new
                {
                    pv.Url,
                    pv.Fecha,
                    pv.IpAddress,
                    pv.Referrer
                })
            });
        })
        .WithName("GetPageViewStats")
        .WithOpenApi()
        .WithSummary("📈 Obtener estadísticas de visitas")
        .WithDescription("Obtiene las estadísticas de visitas de una empresa en un rango de fechas");

        // GET /api/PageViews/queue/stats - Estadísticas de la cola (solo con autenticación, admin)
        protectedGroup.MapGet("/queue/stats", (IPageViewQueueService queueService) =>
        {
            var stats = queueService.GetQueueStats();
            return Results.Ok(new
            {
                queueSize = stats.CurrentQueueSize,
                totalEnqueued = stats.TotalEnqueued,
                totalDequeued = stats.TotalDequeued,
                totalDiscarded = stats.TotalDiscarded,
                lastEnqueueTime = stats.LastEnqueueTime,
                lastDequeueTime = stats.LastDequeueTime,
                percentageFull = Math.Round((stats.CurrentQueueSize / 10000.0) * 100, 2)
            });
        })
        .WithName("GetQueueStats")
        .WithOpenApi()
        .WithSummary("📊 Estadísticas de la cola")
        .WithDescription("Obtiene las estadísticas en tiempo real de la cola de PageViews");

        // POST /api/PageViews/queue/reset - Reset de estadísticas (solo con autenticación, admin)
        protectedGroup.MapPost("/queue/reset", (IPageViewQueueService queueService) =>
        {
            queueService.ResetStats();
            return Results.Ok(new { message = "Queue statistics reset successfully" });
        })
        .WithName("ResetQueueStats")
        .WithOpenApi()
        .WithSummary("🔄 Resetear estadísticas de la cola")
        .WithDescription("Resetea las estadísticas de la cola de PageViews");
    }
}

public record PageViewTrackRequest(
    string CodigoEmpresa, // Código de la empresa (ej: "EMPRESA-001")
    string Url
);
