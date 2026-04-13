using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Queries.GetPaginaByCodigo;
using PedidosBarrio.Application.Queries.GetPaginaMia;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Services;

namespace PedidosBarrio.Api.EndPoint;

public static class PaginaEndpoint
{
    public static void MapPaginaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/Paginas")
                       .WithTags("Páginas - Sitio Web");

        // GET /api/Paginas/{codigo} - Obtener página por código del negocio (público)
        group.MapGet("/{codigo}", ObtenerPaginaByCodigo)
            .WithName("ObtenerPaginaByCodigo")
            .WithOpenApi()
            .Produces<PaginaDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Obtener página por código del negocio")
            .WithDescription("Retorna la página web de un negocio por su código. No requiere autenticación.");

        // GET /api/Paginas/mia - Obtener página de mi empresa (privado)
        group.MapGet("", ObtenerPaginasPorEmpresa)
            .WithName("ObtenerPaginaPorEmpresa")
            .WithOpenApi()
            .RequireAuthorization()
            .Produces<PaginaDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .WithSummary("Obtener página de mi empresa")
            .WithDescription("Retorna la página web de la empresa del usuario autenticado. Requiere autenticación.");


        // POST /api/Paginas - Crear nueva página
        group.MapPost("/", CrearPagina)
            .WithName("CrearPagina")
            .WithOpenApi()
            .RequireAuthorization()
            .Produces<PaginaDto>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .WithSummary("Crear nueva página")
            .WithDescription("Crea una nueva página web con contenido JSONB. El código del negocio se obtiene del token JWT (requiere autenticación)");

    }


    private static async Task<IResult> ObtenerPaginaByCodigo(
        [FromRoute] string codigo,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetPaginaByCodigoQuery(codigo);
        var result = await sender.Send(query, cancellationToken);

        if (result == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(result);
    }

   
    private static async Task<IResult> ObtenerPaginasPorEmpresa(
        IPaginaRepository paginaRepository,
        ICurrentUserService currentUserService,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        try
        {
            Guid empresaId;
            try
            {
                empresaId = currentUserService.GetEmpresaId();
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Unauthorized();
            }
            var pagina = await paginaRepository.GetByCodigoEmpresaAsync(empresaId);

            if (pagina == null)
            {
                return Results.Ok(new PaginaDto());
            }

            return Results.Ok(pagina);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new
            {
                success = false,
                message = "Error al obtener páginas",
                detail = ex.Message
            });
        }
    }

    private static async Task<IResult> CrearPagina(
        [FromBody] CreatePaginaDto request,
        ICurrentUserService currentUserService,
        IPaginaRepository paginaRepository,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        try
        {
            if (request == null)
            {
                return Results.BadRequest(new
                {
                    success = false,
                    message = "Datos de página inválidos"
                });
            }

            if (string.IsNullOrEmpty(request.Contenido))
            {
                return Results.BadRequest(new
                {
                    success = false,
                    message = "Contenido es requerido"
                });
            }

            // Obtener EmpresaID del token JWT
            Guid empresaId;
            try
            {
                empresaId = currentUserService.GetEmpresaId();
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Unauthorized();
            }

            // Verificar si ya existe una página para esta empresa
            var paginaExistente = await paginaRepository.GetByCodigoEmpresaAsync(empresaId);

            if (paginaExistente != null)
            {
                // Actualizar página existente
                paginaExistente.Contenido = request.Contenido;
                paginaExistente.Descripcion = request.Descripcion;
                paginaExistente.FechaActualizacion = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

                var paginaActualizada = await paginaRepository.UpdateAsync(paginaExistente);
                var dtoActualizado = mapper.Map<PaginaDto>(paginaActualizada);
                return Results.Ok(dtoActualizado);
            }

            // Crear nueva página
            var pagina = new Domain.Entities.Pagina(empresaId, request.Contenido)
            {
                Descripcion = request.Descripcion
            };

            var paginaCreada = await paginaRepository.AddAsync(pagina);
            var dto = mapper.Map<PaginaDto>(paginaCreada);
            return Results.Created($"/api/Paginas/{paginaCreada.PaginaID}", dto);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new
            {
                success = false,
                message = "Error al crear página",
                detail = ex.Message
            });
        }
    }
    
}
