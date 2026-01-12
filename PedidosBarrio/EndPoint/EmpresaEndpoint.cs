using MediatR;
using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.Commands.DeleteEmpresa;
using PedidosBarrio.Application.Commands.Login;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Queries.GetAllEmpresas;
using PedidosBarrio.Application.Queries.GetEmpresaById;

namespace PedidosBarrio.Api.EndPoint
{
    public static class EmpresaEndpoint
    {
        public static void MapEmpresaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/Empresas")
                           .WithTags("Empresas");

            // GET /api/Empresas
            group.MapGet("/", async (IMediator mediator) =>
            {
                var empresas = await mediator.Send(new GetAllEmpresasQuery());
                return Results.Ok(empresas);
            })
            .WithName("GetAllEmpresas")
            .WithOpenApi();

            // GET /api/Empresas/{id}
            group.MapGet("/{id:guid}", async (Guid id, IMediator mediator) =>
            {
                var empresa = await mediator.Send(new GetEmpresaByIdQuery(id));
                return empresa is not null ? Results.Ok(empresa) : Results.NotFound();
            })
            .WithName("GetEmpresaById")
            .WithOpenApi();


            // DELETE /api/Empresas/{id}
            group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator) =>
            {
                await mediator.Send(new DeleteEmpresaCommand(id));
                return Results.NoContent();
            })
            .WithName("DeleteEmpresa")
            .WithOpenApi();

            // POST /api/Empresas/Login
            group.MapPost("/Login", async ([FromBody] LoginDto loginDto, IMediator mediator) =>
            {
                var loginResponse = await mediator.Send(new LoginCommand(loginDto.Email, loginDto.Contrasena));
                return Results.Ok(loginResponse);
            })
            .WithName("Login")
            .WithOpenApi();

        }
    }
}
