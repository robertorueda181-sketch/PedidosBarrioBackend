using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.Commands.CreatePedido;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Api.EndPoint;

public static class PedidoEndpoint
{
    public static void MapPedidoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/Pedidos")
                       .WithTags("Pedidos")
                       .RequireAuthorization();

        // POST /api/Pedidos - Crear un nuevo pedido
        group.MapPost("/", CreatePedido)
            .WithName("CreatePedido")
            .WithOpenApi()
            .Produces<CreatePedidoResponse>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithSummary("Crear un nuevo pedido")
            .WithDescription("Crea un nuevo pedido usando el código del negocio. " +
                           "El cliente debe estar autenticado. " +
                           "Reduce el stock de los productos con inventario habilitado.");
    }

    private static async Task<IResult> CreatePedido(
        [FromBody] CreatePedidoDto request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validar que los datos no sean nulos
            if (request == null || string.IsNullOrEmpty(request.Codigo) || 
                request.Cliente == null || request.Productos == null || request.Productos.Count == 0)
            {
                return Results.BadRequest(new
                {
                    success = false,
                    message = "El código del negocio, cliente y al menos un producto son requeridos"
                });
            }

            var command = new CreatePedidoCommand(request);
            var result = await mediator.Send(command, cancellationToken);

            return Results.Created($"/api/Pedidos/{result.PedidoUID}", new
            {
                success = true,
                data = result,
                message = "Pedido creado exitosamente"
            });
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new
            {
                success = false,
                message = "Error al crear el pedido",
                detail = ex.Message
            });
        }
    }
}
