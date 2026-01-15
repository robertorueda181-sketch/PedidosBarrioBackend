using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.Commands.CreateCategoria;
using PedidosBarrio.Application.Commands.DeleteCategoria;
using PedidosBarrio.Application.Commands.UpdateCategoria;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Queries.GetAllCategorias;
using PedidosBarrio.Application.Queries.GetCategoriaById;

namespace PedidosBarrio.Api.EndPoint
{
    public static class CategoriaEndpoint
    {
        public static void MapCategoriaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/Categorias")
                       .WithTags("Categorias")
                       .RequireAuthorization(); // Requiere autenticación para todos los endpoints

        var testGroup = app.MapGroup("/api/TestCategorias")
                           .WithTags("Test Categorias (Sin Auth)")
                           .AllowAnonymous(); // Grupo temporal sin autenticación

            // GET /api/Categorias - Lista categorias por empresa del token
            group.MapGet("/", async (IMediator mediator) =>
            {
                var categorias = await mediator.Send(new GetAllCategoriasQuery());
                return Results.Ok(categorias);
            })
            .WithName("GetAllCategorias")
            .WithOpenApi()
            .WithSummary("Obtiene todas las categorías de la empresa del usuario logueado")
            .WithDescription("Retorna solo CategoriaID, Descripcion y Color de las categorías activas de la empresa");

            // GET /api/Categorias/{id}
            group.MapGet("/{id:int}", async (int id, IMediator mediator) =>
            {
                var categoria = await mediator.Send(new GetCategoriaByIdQuery((short)id));
                return categoria is not null ? Results.Ok(categoria) : Results.NotFound();
            })
            .WithName("GetCategoriaById")
            .WithOpenApi()
            .WithSummary("Obtiene una categoría por ID")
            .WithDescription("Retorna los detalles de una categoría específica");

            // POST /api/Categorias - Agregar nueva categoría (EmpresaID del token)
            group.MapPost("/", async ([FromBody] CreateCategoriaDto createDto, IMediator mediator) =>
            {
                var categoriaDto = await mediator.Send(new CreateCategoriaCommand(
                    createDto.Descripcion, 
                    createDto.Color));
                    
                return Results.Created($"/api/Categorias/{categoriaDto.CategoriaID}", categoriaDto);
            })
            .WithName("CreateCategoria")
            .WithOpenApi()
            .WithSummary("Crea una nueva categoría")
            .WithDescription("Crea una nueva categoría para la empresa del usuario logueado. EmpresaID se obtiene del token JWT");

            // PUT /api/Categorias/{id} - Editar solo Descripcion y Color
            group.MapPut("/{id:int}", async (int id, [FromBody] UpdateCategoriaDto updateDto, IMediator mediator) =>
            {
                var command = new UpdateCategoriaCommand((short)id, updateDto.Descripcion, updateDto.Color);
                await mediator.Send(command);
                return Results.NoContent();
            })
            .WithName("UpdateCategoria")
            .WithOpenApi()
            .WithSummary("Actualiza una categoría")
            .WithDescription("Permite editar solo la Descripcion y Color de una categoría");

            // DELETE /api/Categorias/{id} - Soft delete (convierte Activo a false)
            group.MapDelete("/{id:int}", async (int id, IMediator mediator) =>
            {
                // Convertir int a short para el command
                await mediator.Send(new DeleteCategoriaCommand((short)id));
                return Results.NoContent();
            })
            .WithName("DeleteCategoria")
            .WithOpenApi()
            .WithSummary("Elimina una categoría (soft delete)")
            .WithDescription("Realiza un soft delete estableciendo Activo = false en lugar de eliminar físicamente");

        // ========================================
        // ENDPOINTS TEMPORALES SIN AUTENTICACIÓN PARA TESTING
        // ========================================

        // GET /api/TestCategorias - Para testing sin JWT
        testGroup.MapGet("/", async (IMediator mediator) =>
        {
            // Simular empresa ID hardcoded para testing
            var categorias = await mediator.Send(new GetAllCategoriasQuery());
            return Results.Ok(categorias);
        })
        .WithName("GetAllCategoriasTest")
        .WithOpenApi()
        .WithSummary("⚠️ TEMPORAL - Obtiene categorías sin autenticación")
        .WithDescription("SOLO PARA TESTING - No usar en producción");

        // POST /api/TestCategorias - Para testing sin JWT  
        testGroup.MapPost("/", async ([FromBody] CreateCategoriaDto createDto, IMediator mediator) =>
        {
            // Por ahora usará un GUID hardcoded como empresa
            var categoriaDto = await mediator.Send(new CreateCategoriaCommand(
                createDto.Descripcion, 
                createDto.Color));
                
            return Results.Created($"/api/TestCategorias/{categoriaDto.CategoriaID}", categoriaDto);
        })
        .WithName("CreateCategoriaTest")
        .WithOpenApi()
        .WithSummary("⚠️ TEMPORAL - Crea categoría sin autenticación")
        .WithDescription("SOLO PARA TESTING - No usar en producción");
    }
    }
}