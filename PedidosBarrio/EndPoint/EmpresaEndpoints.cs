using MediatR;
using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.Commands.CreateEmpresa;
using PedidosBarrio.Application.Commands.DeleteEmpresa;
using PedidosBarrio.Application.Commands.UpdateCompany;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Queries.GetEmpresaById;

namespace PedidosBarrio.Api.EndPoint
{
    public static class EmpresaEndpoints
    {
        public static void MapEmpresaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/empresas") // Agrupa endpoints con un prefijo común
                           .WithTags("Empresas") // Para Swagger
                           .WithOpenApi(); // Para Swagger

            // GET /api/empresas
            //group.MapGet("/", async (IMediator mediator) =>
            //{
            //    var empresas = await mediator.Send(new GetAllCompaniesQuery());
            //    return Results.Ok(empresas);
            //})
            //.WithName("GetAllEmpresas");

            // GET /api/empresas/{id}
            group.MapGet("/{id:guid}", async (Guid id, IMediator mediator) =>
            {
                var empresa = await mediator.Send(new GetCompanyByIdQuery(id));
                return empresa is not null ? Results.Ok(empresa) : Results.NotFound();
            })
            .WithName("GetEmpresaById");

            // POST /api/empresas
            group.MapPost("/", async ([FromBody] CreateCompanyDto createDto, IMediator mediator) =>
            {
                var empresaDto = await mediator.Send(new CreateCompanyCommand(createDto));
                return Results.CreatedAtRoute("GetEmpresaById", new { id = empresaDto.Id }, empresaDto);
            })
            .WithName("CreateEmpresa");

            // PUT /api/empresas/{id}
            group.MapPut("/{id:guid}", async (Guid id, [FromBody] CreateCompanyDto updateDto, IMediator mediator) =>
            {
                await mediator.Send(new UpdateCompanyCommand(id, updateDto));
                return Results.NoContent();
            })
            .WithName("UpdateEmpresa");

            // DELETE /api/empresas/{id}
            group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator) =>
            {
                await mediator.Send(new DeleteCompanyCommand(id));
                return Results.NoContent();
            })
            .WithName("DeleteEmpresa");
        }
    }
}