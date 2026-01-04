using MediatR;
using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.Commands.CreateImagen;
using PedidosBarrio.Application.Commands.DeleteImagen;
using PedidosBarrio.Application.Commands.UpdateImagen;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Queries.GetAllImagenes;
using PedidosBarrio.Application.Queries.GetImagenById;
using PedidosBarrio.Application.Queries.GetImagenesByProducto;

namespace PedidosBarrio.Api.EndPoint
{
    public static class ImagenEndpoint
    {
        public static void MapImagenEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/Imagenes")
                           .WithTags("Imagenes");

            // GET /api/Imagenes
            group.MapGet("/", async (IMediator mediator) =>
            {
                var imagenes = await mediator.Send(new GetAllImagenesQuery());
                return Results.Ok(imagenes);
            })
            .WithName("GetAllImagenes")
            .WithOpenApi();

            // GET /api/Imagenes/{id}
            group.MapGet("/{id:int}", async (int id, IMediator mediator) =>
            {
                var imagen = await mediator.Send(new GetImagenByIdQuery(id));
                return imagen is not null ? Results.Ok(imagen) : Results.NotFound();
            })
            .WithName("GetImagenById")
            .WithOpenApi();

            // GET /api/Imagenes/producto/{productoId}
            group.MapGet("/id/{externalId:int}", async (int externalId, string tipo, IMediator mediator) =>
            {
                var imagenes = await mediator.Send(new GetImagenesByProductoQuery(externalId,tipo));
                return Results.Ok(imagenes);
            })
            .WithName("GetImagenesById")
            .WithOpenApi();

            // POST /api/Imagenes
            group.MapPost("/", async ([FromBody] CreateImagenDto createDto, IMediator mediator) =>
            {
                var imagenDto = await mediator.Send(new CreateImagenCommand(createDto.ProductoID, createDto.URLImagen, createDto.Descripcion));
                return Results.Created($"/api/Imagenes/{imagenDto.ImagenID}", imagenDto);
            })
            .WithName("CreateImagen")
            .WithOpenApi();

            // PUT /api/Imagenes/{id}
            group.MapPut("/{id:int}", async (int id, [FromBody] ImagenDto updateDto, IMediator mediator) =>
            {
                var command = new UpdateImagenCommand(id, updateDto.ProductoID, updateDto.URLImagen, updateDto.Descripcion);
                await mediator.Send(command);
                return Results.NoContent();
            })
            .WithName("UpdateImagen")
            .WithOpenApi();

            // DELETE /api/Imagenes/{id}
            group.MapDelete("/{id:int}", async (int id, IMediator mediator) =>
            {
                await mediator.Send(new DeleteImagenCommand(id));
                return Results.NoContent();
            })
            .WithName("DeleteImagen")
            .WithOpenApi();
        }
    }
}
