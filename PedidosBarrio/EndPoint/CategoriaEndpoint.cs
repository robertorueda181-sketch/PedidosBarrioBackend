using MediatR;
using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.Commands.CreateProducto;
using PedidosBarrio.Application.Commands.DeleteProducto;
using PedidosBarrio.Application.Commands.UpdateProducto;
using PedidosBarrio.Application.Commands.UpdateProductoVisible;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Queries.GetAllCategorias;
using PedidosBarrio.Application.Queries.GetAllProductos;
using PedidosBarrio.Application.Queries.GetProductoById;

namespace PedidosBarrio.Api.EndPoint
{
    public static class CategoriaEndpoint
    {
        public static void MapCategoriaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/Categorias")
                           .WithTags("Categorias")
                           .RequireAuthorization();


            var groupProductos = app.MapGroup("/api/Productos")
                           .WithTags("Productos")
                           .RequireAuthorization();

            // ===== ENDPOINT: OBTENER SOLO CATEGORÍAS =====
            group.MapGet("/", async (IMediator mediator) =>
            {
                var query = new GetAllCategoriasQuery();
                var result = await mediator.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetOnlyCategorias")
            .WithOpenApi()
            .WithSummary("📂 Obtener todas las categorías")
            .WithDescription("Retorna solo las categorías de la empresa del usuario logueado");

            // ===== ENDPOINT: OBTENER TODOS LOS PRODUCTOS CON IMÁGENES Y PRECIOS =====
            groupProductos.MapGet("/public/{codigo}", async (IMediator mediator, string codigo) =>
            {
                var query = new GetAllProductosQuery(codigo);
                var result = await mediator.Send(query);
                return Results.Ok(result);
            })
              .WithName("GetAllProductosPublics")
              .WithOpenApi()
              .WithSummary("🛍️ Obtener productos (público)")
              .WithDescription("Retorna todos los productos sin autenticación")
              .AllowAnonymous();

            groupProductos.MapGet("/", async (
               IMediator mediator,
               int? count) =>
                {
                    var query = new GetAllProductosQuery();
                    query.CantReg = count;
                    var result = await mediator.Send(query);

                    return Results.Ok(result);
                })
           .WithName("GetAllProductos")
           .WithOpenApi()
           .WithSummary("🛍️ Obtener productos")
            .WithDescription("Retorna todos los productos");

            //group.MapGet("/{id:int}", async (int id, IMediator mediator) =>
            //{
            //    var categoria = await mediator.Send(new GetCategoriaByIdQuery((short)id));
            //    return categoria is not null ? Results.Ok(categoria) : Results.NotFound();
            //})
            //.WithName("GetCategoriaById")
            //.WithOpenApi()
            //.WithSummary("📂 Obtener categoría por ID")
            //.WithDescription("Retorna los detalles de una categoría específica");


            // ===== ENDPOINTS DE PRODUCTOS =====
            groupProductos.MapGet("/{id:int}", async (int id, IMediator mediator) =>
            {
                var query = new GetProductoByIdQuery(id);
                var result = await mediator.Send(query);
                return result is not null ? Results.Ok(result) : Results.NotFound();
            })
            .WithName("GetProductoById")
            .WithOpenApi()
            .WithSummary("🛍️ Obtener producto por ID")
            .WithDescription("Retorna los detalles de un producto específico incluyendo presentaciones y precios");

            groupProductos.MapPost("/", async (
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

            groupProductos.MapPut("/{id:int}", async (
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

            groupProductos.MapDelete("/{id:int}", async (int id, IMediator mediator) =>
            {
                var command = new DeleteProductoCommand(id);
                var result = await mediator.Send(command);
                return Results.Ok(new { success = result, message = "Producto eliminado correctamente" });
            })
            .WithName("DeleteProducto")
            .WithOpenApi()
            .WithSummary("🗑️ Eliminar producto")
            .WithDescription("Elimina un producto verificando que pertenezca a la empresa");

            groupProductos.MapPatch("/visible", async (
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