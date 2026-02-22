using MediatR;
using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Queries.GetDashboard;
using PedidosBarrio.Application.Services;

namespace PedidosBarrio.Api.EndPoint
{
    public static class DashboardEndpoint
    {
        public static void MapDashboardEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/Dashboard")
                .WithTags("Dashboard")
                .RequireAuthorization(); // Requiere autenticación

            // GET /api/Dashboard - Obtener dashboard de la empresa actual
            group.MapGet("/", async (
                IMediator mediator,
                ICurrentUserService currentUserService) =>
            {
                try
                {
                    var empresaId = currentUserService.GetEmpresaId();

                    if (empresaId == Guid.Empty)
                    {
                        return Results.BadRequest(new { error = "No se pudo identificar la empresa del usuario" });
                    }

                    var dashboard = await mediator.Send(new GetDashboardQuery(empresaId));
                    return Results.Ok(dashboard);
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError,
                        title: "Error al obtener datos del dashboard"
                    );
                }
            })
            .WithName("GetDashboard")
            .WithOpenApi()
            .WithSummary("📊 Obtener Dashboard")
            .WithDescription(@"
Retorna un dashboard con:
- **Cantidad de productos**: Total de productos activos y aprobados de la empresa
- **Vistas hoy**: Cantidad de visitas (PageViews) registradas en el día de hoy
- **Información de suscripción**: Datos de la suscripción activa (nivel, monto, fechas, etc.)
- **Estadísticas por mes**: Vistas agrupadas por mes para los últimos 12 meses

Requiere autenticación JWT. La empresa se obtiene automáticamente del usuario autenticado.
            ")
            .Produces<DashboardDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);
        }
    }
}
