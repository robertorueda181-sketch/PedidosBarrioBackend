using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using PedidosBarrio.Application.Queries.GetMenusByEmpresa;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Api.EndPoint
{
    public static class ConfiguracionEndpoint
    {
        public static void MapConfiguracionEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/configuracion").WithTags("Configuracion");

            // Endpoint que requiere empresaId como parámetro
            group.MapGet("/menus/{empresaId}", GetMenusByEmpresa)
                .WithName("GetMenusByEmpresa")
                .WithOpenApi()
                .Produces<IEnumerable<MenuDto>>(StatusCodes.Status200OK)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                .WithSummary("Obtiene los menus por empresa");

            // Nuevo endpoint que obtiene la empresaId del token JWT
            group.MapGet("/menus", GetMenusFromToken)
                .RequireAuthorization()
                .WithName("GetMenusFromToken")
                .WithOpenApi()
                .Produces<IEnumerable<MenuDto>>(StatusCodes.Status200OK)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                .WithDescription("Obtiene los menús de configuración de la empresa autenticada desde el token JWT")
                .WithSummary("Menús desde Token JWT");
        }

        private static async Task<IResult> GetMenusByEmpresa([FromRoute] Guid empresaId, IMediator mediator)
        {
            var result = await mediator.Send(new GetMenusByEmpresaQuery(empresaId));
            return Results.Ok(result);
        }

        private static async Task<IResult> GetMenusFromToken(HttpContext context, IMediator mediator)
        {
            try
            {
                // Obtener el claim de EmpresaID del token JWT
                var empresaIdClaim = context.User.FindFirst("EmpresaID");
                
                if (empresaIdClaim == null)
                {
                    return Results.BadRequest(new { error = "No se encontró la empresa ID en el token" });
                }

                if (!Guid.TryParse(empresaIdClaim.Value, out Guid empresaId))
                {
                    return Results.BadRequest(new { error = "La empresa ID en el token no es válida" });
                }

                var result = await mediator.Send(new GetMenusByEmpresaQuery(empresaId));
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: "Error interno del servidor",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }
    }
}