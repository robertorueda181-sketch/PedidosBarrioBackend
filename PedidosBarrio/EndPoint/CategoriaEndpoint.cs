using MediatR;
using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.Commands.CreateProducto;
using PedidosBarrio.Application.Commands.DeleteProducto;
using PedidosBarrio.Application.Commands.UpdateProducto;
using PedidosBarrio.Application.Commands.UpdateProductoVisible;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Queries.GetProductoById;

namespace PedidosBarrio.Api.EndPoint
{
    public static class CategoriaEndpoint
    {
        public static void MapCategoriaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/Categorias")
                           .WithTags("Categorias y Productos")
                           .RequireAuthorization();

            // ===== ENDPOINT: OBTENER SOLO CATEGORÍAS =====
            group.MapGet("/getAll", async (IMediator mediator) =>
            {
                var query = new GetOnlyCategoriasQuery();
                var result = await mediator.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetOnlyCategorias")
            .WithOpenApi()
            .WithSummary("📂 Obtener todas las categorías")
            .WithDescription("Retorna solo las categorías de la empresa del usuario logueado");

            // ===== ENDPOINTS DE PRODUCTOS =====
            group.MapGet("/productos/{id:int}", async (int id, IMediator mediator) =>
            {
                var query = new GetProductoByIdQuery(id);
                var result = await mediator.Send(query);
                return result is not null ? Results.Ok(result) : Results.NotFound();
            })
            .WithName("GetProductoById")
            .WithOpenApi()
            .WithSummary("🛍️ Obtener producto por ID")
            .WithDescription("Retorna los detalles de un producto específico incluyendo presentaciones y precios");

            group.MapPost("/productos", async (
                [FromBody] CreateProductoDto productoDto,
                IMediator mediator) =>
            {
                var command = new CreateProductoCommand(productoDto);
                var result = await mediator.Send(command);
                return Results.Created($"/api/categorias/productos/{result.ProductoID}", result);
            })
            .WithName("CreateProducto")
            .WithOpenApi()
            .WithSummary("🛍️ Crear nuevo producto")
            .WithDescription("Crea un nuevo producto verificando que la categoría pertenezca a la empresa");

            group.MapPut("/productos/{id:int}", async (
                int id,
                [FromBody] UpdateProductoDto productoDto,
                IMediator mediator) =>
            {
                var command = new UpdateProductoCommand(id, productoDto);
                var result = await mediator.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateProducto")
            .WithOpenApi()
            .WithSummary("✏️ Actualizar producto")
            .WithDescription("Actualiza un producto existente verificando que pertenezca a la empresa");

            group.MapDelete("/productos/{id:int}", async (int id, IMediator mediator) =>
            {
                var command = new DeleteProductoCommand(id);
                var result = await mediator.Send(command);
                return Results.Ok(new { success = result, message = "Producto eliminado correctamente" });
            })
            .WithName("DeleteProducto")
            .WithOpenApi()
            .WithSummary("🗑️ Eliminar producto")
            .WithDescription("Elimina un producto verificando que pertenezca a la empresa");

            group.MapPatch("/productos/visible", async (
                [FromBody] UpdateProductoVisibleDto dto,
                IMediator mediator) =>
            {
                var command = new UpdateProductoVisibleCommand(dto.ProductoID, dto.Visible);
                var result = await mediator.Send(command);
                return Results.Ok(new { success = result, message = "Visibilidad del producto actualizada" });
            })
            .WithName("UpdateProductoVisible")
            .WithOpenApi()
            .WithSummary("👁️ Cambiar visibilidad de producto")
            .WithDescription("Permite activar o desactivar la visibilidad de un producto de la empresa");
        }
    }
}