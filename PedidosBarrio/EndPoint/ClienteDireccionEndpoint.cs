using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Api.EndPoint;

public static class ClienteDireccionEndpoint
{
    public static void MapClienteDireccionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/Clientes/Direcciones")
                       .WithTags("Direcciones - Clientes")
                       .RequireAuthorization();

        // GET /api/Clientes/Direcciones - Obtener todas las direcciones del cliente autenticado
        group.MapGet("/", ObtenerDirecciones)
            .WithName("ObtenerDireccionesCliente")
            .WithOpenApi()
            .Produces<IEnumerable<ClienteDireccionDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .WithSummary("Obtener todas las direcciones del cliente")
            .WithDescription("Retorna todas las direcciones activas del cliente autenticado, extraído del token JWT");

        // GET /api/Clientes/Direcciones/principal - Obtener dirección principal
        group.MapGet("/principal", ObtenerDireccionPrincipal)
            .WithName("ObtenerDireccionPrincipal")
            .WithOpenApi()
            .Produces<ClienteDireccionDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .WithSummary("Obtener dirección principal del cliente")
            .WithDescription("Retorna la dirección marcada como principal (clienteId del token JWT)");

        // GET /api/Clientes/Direcciones/{direccionId} - Obtener una dirección específica
        group.MapGet("/{direccionId:guid}", ObtenerDireccionPorId)
            .WithName("ObtenerDireccionPorId")
            .WithOpenApi()
            .Produces<ClienteDireccionDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .WithSummary("Obtener dirección específica")
            .WithDescription("Retorna los detalles de una dirección específica verificando que pertenezca al cliente autenticado");

        // POST /api/Clientes/Direcciones - Crear nueva dirección
        group.MapPost("/", CrearDireccion)
            .WithName("CrearDireccion")
            .WithOpenApi()
            .Produces<ClienteDireccionDto>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .WithSummary("Crear nueva dirección")
            .WithDescription("Crea una nueva dirección para el cliente autenticado. La clienteId se extrae del token JWT");

        // PUT /api/Clientes/Direcciones/{direccionId} - Actualizar dirección
        group.MapPut("/{direccionId:guid}", ActualizarDireccion)
            .WithName("ActualizarDireccion")
            .WithOpenApi()
            .Produces<ClienteDireccionDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .WithSummary("Actualizar dirección")
            .WithDescription("Actualiza los datos de una dirección existente del cliente autenticado");

        // DELETE /api/Clientes/Direcciones/{direccionId} - Eliminar dirección (soft delete)
        group.MapDelete("/{direccionId:guid}", EliminarDireccion)
            .WithName("EliminarDireccion")
            .WithOpenApi()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .WithSummary("Eliminar dirección")
            .WithDescription("Elimina (soft delete) una dirección del cliente autenticado");

        // POST /api/Clientes/Direcciones/{direccionId}/establecer-principal - Establecer como principal
        group.MapPost("/{direccionId:guid}/establecer-principal", EstablecerDireccionPrincipal)
            .WithName("EstablecerDireccionPrincipal")
            .WithOpenApi()
            .Produces<ClienteDireccionDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .WithSummary("Establecer dirección como principal")
            .WithDescription("Marca una dirección como principal (desmarca las otras automáticamente)");
    }

    private static async Task<IResult> ObtenerDirecciones(
        IClienteDireccionRepository repository,
        ICurrentUserService currentUserService,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        try
        {
            var clienteId = currentUserService.GetClienteId();

            var direcciones = await repository.GetByClienteIdAsync(clienteId);

            if (direcciones == null || !direcciones.Any())
            {
                return Results.Ok(new List<ClienteDireccionDto>());
            }

            var dto = mapper.Map<IEnumerable<ClienteDireccionDto>>(direcciones);
            return Results.Ok(dto);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Results.Unauthorized();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new
            {
                success = false,
                message = "Error al obtener direcciones",
                detail = ex.Message
            });
        }
    }

    private static async Task<IResult> ObtenerDireccionPrincipal(
        IClienteDireccionRepository repository,
        ICurrentUserService currentUserService,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        try
        {
            var clienteId = currentUserService.GetClienteId();

            var direccion = await repository.GetPrincipalByClienteIdAsync(clienteId);

            if (direccion == null)
            {
                return Results.NotFound(new
                {
                    success = false,
                    message = "No se encontró dirección principal para el cliente"
                });
            }

            var dto = mapper.Map<ClienteDireccionDto>(direccion);
            return Results.Ok(dto);
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new
            {
                success = false,
                message = "Error al obtener dirección principal",
                detail = ex.Message
            });
        }
    }

    private static async Task<IResult> ObtenerDireccionPorId(
        [FromRoute] Guid direccionId,
        IClienteDireccionRepository repository,
        ICurrentUserService currentUserService,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        try
        {
            var clienteId = currentUserService.GetClienteId();

            var direccion = await repository.GetByIdAsync(direccionId);

            if (direccion == null || direccion.ClienteID != clienteId)
            {
                return Results.NotFound(new
                {
                    success = false,
                    message = "Dirección no encontrada"
                });
            }

            var dto = mapper.Map<ClienteDireccionDto>(direccion);
            return Results.Ok(dto);
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new
            {
                success = false,
                message = "Error al obtener dirección",
                detail = ex.Message
            });
        }
    }

    private static async Task<IResult> CrearDireccion(
        [FromBody] CreateClienteDireccionDto request,
        IClienteDireccionRepository repository,
        ICurrentUserService currentUserService,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        try
        {
            var clienteId = currentUserService.GetClienteId();

            if (request == null)
            {
                return Results.BadRequest(new
                {
                    success = false,
                    message = "Datos de dirección inválidos"
                });
            }

            if (string.IsNullOrEmpty(request.Nombre) || string.IsNullOrEmpty(request.DireccionTexto))
            {
                return Results.BadRequest(new
                {
                    success = false,
                    message = "Nombre y dirección son requeridos"
                });
            }

            // Crear la nueva dirección
            var direccion = mapper.Map<Domain.Entities.ClienteDireccion>(request);
            direccion.ClienteID = clienteId;
            direccion.ClienteDireccionID = Guid.NewGuid();
            direccion.FechaCreacion = DateTime.UtcNow;
            direccion.Activa = true;

            // Si es la primera dirección, marcarla como principal
            var hasDirecciones = await repository.HasDireccionesAsync(clienteId);
            if (!hasDirecciones)
            {
                direccion.EsPrincipal = true;
            }

            await repository.AddAsync(direccion);

            var dto = mapper.Map<ClienteDireccionDto>(direccion);
            return Results.Created($"/api/Clientes/Direcciones/{direccion.ClienteDireccionID}", dto);
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new
            {
                success = false,
                message = "Error al crear dirección",
                detail = ex.Message
            });
        }
    }

    private static async Task<IResult> ActualizarDireccion(
        [FromRoute] Guid direccionId,
        [FromBody] UpdateClienteDireccionDto request,
        IClienteDireccionRepository repository,
        ICurrentUserService currentUserService,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        try
        {
            var clienteId = currentUserService.GetClienteId();

            if (request == null)
            {
                return Results.BadRequest(new
                {
                    success = false,
                    message = "Datos de dirección inválidos"
                });
            }

            var direccion = await repository.GetByIdAsync(direccionId);

            if (direccion == null || direccion.ClienteID != clienteId)
            {
                return Results.NotFound(new
                {
                    success = false,
                    message = "Dirección no encontrada"
                });
            }

            mapper.Map(request, direccion);
            direccion.FechaActualizacion = DateTime.UtcNow;

            await repository.UpdateAsync(direccion);

            var dto = mapper.Map<ClienteDireccionDto>(direccion);
            return Results.Ok(dto);
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new
            {
                success = false,
                message = "Error al actualizar dirección",
                detail = ex.Message
            });
        }
    }

    private static async Task<IResult> EliminarDireccion(
        [FromRoute] Guid direccionId,
        IClienteDireccionRepository repository,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        try
        {
            var clienteId = currentUserService.GetClienteId();

            var direccion = await repository.GetByIdAsync(direccionId);

            if (direccion == null || direccion.ClienteID != clienteId)
            {
                return Results.NotFound(new
                {
                    success = false,
                    message = "Dirección no encontrada"
                });
            }

            await repository.DeleteAsync(direccionId);

            return Results.NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new
            {
                success = false,
                message = "Error al eliminar dirección",
                detail = ex.Message
            });
        }
    }

    private static async Task<IResult> EstablecerDireccionPrincipal(
        [FromRoute] Guid direccionId,
        IClienteDireccionRepository repository,
        ICurrentUserService currentUserService,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        try
        {
            var clienteId = currentUserService.GetClienteId();

            var direccion = await repository.GetByIdAsync(direccionId);

            if (direccion == null || direccion.ClienteID != clienteId)
            {
                return Results.NotFound(new
                {
                    success = false,
                    message = "Dirección no encontrada"
                });
            }

            await repository.SetAsPrincipalAsync(direccionId);

            // Recargar la dirección para devolver datos actualizados
            var direccionActualizada = await repository.GetByIdAsync(direccionId);
            var dto = mapper.Map<ClienteDireccionDto>(direccionActualizada);

            return Results.Ok(dto);
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new
            {
                success = false,
                message = "Error al establecer dirección principal",
                detail = ex.Message
            });
        }
    }
}

      