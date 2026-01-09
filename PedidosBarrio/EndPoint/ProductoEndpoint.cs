using MediatR;
using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.Commands.CreateProducto;
using PedidosBarrio.Application.Commands.DeleteProducto;
using PedidosBarrio.Application.Commands.UpdateProducto;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Queries.GetAllProductos;
using PedidosBarrio.Application.Queries.GetProductoById;
using PedidosBarrio.Application.Queries.GetProductosByEmpresa;

namespace PedidosBarrio.Api.EndPoint
{
    public static class ProductoEndpoint
    {
        public static void MapProductoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/Productos")
                           .WithTags("Productos");

            // GET /api/Productos
            group.MapGet("/", async (IMediator mediator) =>
            {
                var productos = await mediator.Send(new GetAllProductosQuery());
                return Results.Ok(productos);
            })
            .WithName("GetAllProductos")
            .WithOpenApi();

            // GET /api/Productos/{id}
            group.MapGet("/{id:int}", async (int id, IMediator mediator) =>
            {
                var producto = await mediator.Send(new GetProductoByIdQuery(id));
                return producto is not null ? Results.Ok(producto) : Results.NotFound();
            })
            .WithName("GetProductoById")
            .WithOpenApi();

            // GET /api/Productos/empresa/{empresaId}
            group.MapGet("/empresa/{empresaId:int}", async (Guid empresaId, IMediator mediator) =>
            {
                var productos = await mediator.Send(new GetProductosByEmpresaQuery(empresaId));
                return Results.Ok(productos);
            })
            .WithName("GetProductosByEmpresa")
            .WithOpenApi();

            // POST /api/Productos
            group.MapPost("/", async ([FromBody] CreateProductoDto createDto, IMediator mediator) =>
            {
                var productoDto = await mediator.Send(new CreateProductoCommand(createDto.EmpresaID, createDto.Nombre, createDto.Descripcion));
                return Results.Created($"/api/Productos/{productoDto.ProductoID}", productoDto);
            })
            .WithName("CreateProducto")
            .WithOpenApi();

            // PUT /api/Productos/{id}
            group.MapPut("/{id:int}", async (int id, [FromBody] ProductoDto updateDto, IMediator mediator) =>
            {
                var command = new UpdateProductoCommand(id, updateDto.EmpresaID, updateDto.Nombre, updateDto.Descripcion);
                await mediator.Send(command);
                return Results.NoContent();
            })
            .WithName("UpdateProducto")
            .WithOpenApi();

            // DELETE /api/Productos/{id}
            group.MapDelete("/{id:int}", async (int id, IMediator mediator) =>
            {
                await mediator.Send(new DeleteProductoCommand(id));
                return Results.NoContent();
            })
            .WithName("DeleteProducto")
            .WithOpenApi();
        }
    }
}
