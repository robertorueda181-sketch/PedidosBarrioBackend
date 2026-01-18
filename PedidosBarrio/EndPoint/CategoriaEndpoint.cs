using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.Commands.CreateCategoria;
using PedidosBarrio.Application.Commands.CreateProducto;
using PedidosBarrio.Application.Commands.DeleteCategoria;
using PedidosBarrio.Application.Commands.DeleteProducto;
using PedidosBarrio.Application.Commands.UpdateCategoria;
using PedidosBarrio.Application.Commands.UpdateProducto;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Queries.GetAllCategorias;
using PedidosBarrio.Application.Queries.GetCategoriaById;
using PedidosBarrio.Application.Queries.GetCombinedData;
using PedidosBarrio.Infrastructure.Authorization;

namespace PedidosBarrio.Api.EndPoint
{
    public static class CategoriaEndpoint
    {
        public static void MapCategoriaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/Categorias")
                           .WithTags("Categorias y Productos")
                           .RequireAuthorization();

            // ===== ENDPOINT PRINCIPAL: OBTENER CATEGORÍAS Y PRODUCTOS COMBINADOS =====
            group.MapGet("/getAll", async (IMediator mediator) =>
            {
                var query = new GetCombinedDataQuery();
                var result = await mediator.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetCombinedData")
            .WithOpenApi()
            .WithSummary("📋 Obtener categorías y productos de la empresa")
            .WithDescription("Retorna todas las categorías y productos que pertenecen a la empresa del usuario logueado");

            // ===== ENDPOINTS DE CATEGORÍAS =====
            group.MapGet("/", async (IMediator mediator) =>
            {
                var categorias = await mediator.Send(new GetAllCategoriasQuery());
                return Results.Ok(categorias);
            })
            .WithName("GetAllCategorias")
            .WithOpenApi()
            .WithSummary("📂 Obtener todas las categorías")
            .WithDescription("Retorna solo las categorías de la empresa del usuario logueado");

            group.MapGet("/{id:int}", async (int id, IMediator mediator) =>
            {
                var categoria = await mediator.Send(new GetCategoriaByIdQuery((short)id));
                return categoria is not null ? Results.Ok(categoria) : Results.NotFound();
            })
            .WithName("GetCategoriaById")
            .WithOpenApi()
            .WithSummary("📂 Obtener categoría por ID")
            .WithDescription("Retorna los detalles de una categoría específica");

            group.MapPost("/", async ([FromBody] CreateCategoriaDto createDto, IMediator mediator) =>
            {
                var categoriaDto = await mediator.Send(new CreateCategoriaCommand(
                    createDto.Descripcion, 
                    createDto.Color));
                    
                return Results.Created($"/api/Categorias/{categoriaDto.CategoriaID}", categoriaDto);
            })
            .WithName("CreateCategoria")
            .WithOpenApi()
            .WithSummary("✅ Crear nueva categoría")
            .WithDescription("Crea una nueva categoría para la empresa del usuario logueado");

            group.MapPut("/{id:int}", async (int id, [FromBody] UpdateCategoriaDto updateDto, IMediator mediator) =>
            {
                var command = new UpdateCategoriaCommand((short)id, updateDto.Descripcion, updateDto.Color);
                await mediator.Send(command);
                return Results.NoContent();
            })
            .WithName("UpdateCategoria")
            .WithOpenApi()
            .WithSummary("✏️ Actualizar categoría")
            .WithDescription("Actualiza una categoría existente verificando que pertenezca a la empresa");

            group.MapDelete("/{id:int}", async (int id, IMediator mediator) =>
            {
                await mediator.Send(new DeleteCategoriaCommand((short)id));
                return Results.NoContent();
            })
            .WithName("DeleteCategoria")
            .WithOpenApi()
            .WithSummary("🗑️ Eliminar categoría")
            .WithDescription("Elimina una categoría verificando que pertenezca a la empresa");

            // ===== ENDPOINTS DE PRODUCTOS =====
            group.MapPost("/productos", async (
                [FromBody] CreateProductoDto productoDto,
                IMediator mediator,
                IValidator<CreateProductoDto> validator) =>
            {
                var validationResult = await validator.ValidateAsync(productoDto);
                if (!validationResult.IsValid)
                {
                    return Results.BadRequest(new
                    {
                        error = "Datos de entrada inválidos",
                        errors = validationResult.Errors.Select(e => new
                        {
                            field = e.PropertyName,
                            message = e.ErrorMessage
                        })
                    });
                }

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
                IMediator mediator,
                IValidator<UpdateProductoDto> validator) =>
            {
                var validationResult = await validator.ValidateAsync(productoDto);
                if (!validationResult.IsValid)
                {
                    return Results.BadRequest(new
                    {
                        error = "Datos de entrada inválidos",
                        errors = validationResult.Errors.Select(e => new
                        {
                            field = e.PropertyName,
                            message = e.ErrorMessage
                        })
                    });
                }

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
        }
    }
}