using MediatR;
using PedidosBarrio.Application.Commands.CreateNotificacionApp;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Api.EndPoint
{
    public static class NotificacionAppEndpoint
    {
        public static void MapNotificacionAppEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/NotificacionApp").WithTags("NotificacionApp");

            group.MapPost("/crear", async (CreateNotificacionAppCommand command, IMediator mediator) =>
            {
                var id = await mediator.Send(command);
                return Results.Created($"/api/NotificacionApp/{id}", new { Id = id });
            })
            .WithName("CreateNotificacionApp")
            .AllowAnonymous() // Dependiendo si quieres autenticación
            .WithOpenApi();
        }
    }
}
