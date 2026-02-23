using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using PedidosBarrio.Application.Commands.ClienteAuth;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Api.EndPoint;

public static class ClienteAuthEndpoint
{
    public static void MapClienteAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/Clientes/Auth")
                       .WithTags("Autenticación - Clientes");

        // POST /api/Clientes/Auth/Registro - Registrar nuevo cliente
        group.MapPost("/Registro", Registro)
            .WithName("ClienteRegistro")
            .WithOpenApi()
            .Produces<ClienteAuthResponseDto>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithSummary("Registrar nuevo cliente")
            .WithDescription("Registra un nuevo cliente con DNI, nombres y contraseña");

            // POST /api/Clientes/Auth/Login - Login de cliente
            group.MapPost("/Login", Login)
                .WithName("ClienteLogin")
                .WithOpenApi()
                .Produces<ClienteAuthResponseDto>(StatusCodes.Status200OK)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                .WithSummary("Login de cliente")
                .WithDescription("Autentica un cliente existente con DNI y contraseña");

            // POST /api/Clientes/Auth/GoogleAuth - Autenticación con Google (Login o Registro)
            group.MapPost("/GoogleAuth", GoogleAuth)
                .WithName("ClienteGoogleAuth")
                .WithOpenApi()
                .Produces<ClienteAuthResponseDto>(StatusCodes.Status200OK)
                .Produces<ClienteAuthResponseDto>(StatusCodes.Status201Created)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                .WithSummary("Autenticación con Google (Login o Registro)")
                .WithDescription("Autentica un cliente existente o registra uno nuevo usando su Google ID token. Retorna 201 si es nuevo cliente, 200 si es existente");
        }

    private static async Task<IResult> Registro(
        [FromBody] ClienteRegistroDto request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        try
        {
            if (request == null)
            {
                return Results.BadRequest(new
                {
                    success = false,
                    message = "Datos de registro inválidos"
                });
            }

            // Validar que tenga DNI o Email
            var tieneDni = !string.IsNullOrEmpty(request.DNI);
            var tieneEmail = !string.IsNullOrEmpty(request.Email);

            if (!tieneDni && !tieneEmail)
            {
                return Results.BadRequest(new
                {
                    success = false,
                    message = "DNI o Email es requerido"
                });
            }

            // Validar nombres y contraseña
            if (string.IsNullOrEmpty(request.Nombres) || string.IsNullOrEmpty(request.Contrasena))
            {
                return Results.BadRequest(new
                {
                    success = false,
                    message = "Nombres y contraseña son requeridos"
                });
            }

            var command = new ClienteRegistroCommand(request);
            var result = await mediator.Send(command, cancellationToken);

            if (!result.Success)
            {
                return Results.BadRequest(result);
            }

            return Results.Created($"/api/Clientes/{result.Data?.ClienteID}", result);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new
            {
                success = false,
                message = "Error al registrar cliente",
                detail = ex.Message
            });
        }
    }

    private static async Task<IResult> Login(
        [FromBody] ClienteLoginDto request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        try
        {
            if (request == null)
            {
                return Results.BadRequest(new
                {
                    success = false,
                    message = "Datos de login inválidos"
                });
            }

            // Validar que tenga DNI o Email
            var tieneDni = !string.IsNullOrEmpty(request.DNI);
            var tieneEmail = !string.IsNullOrEmpty(request.Email);
            var tieneGoogle = !string.IsNullOrEmpty(request.IdToken);

            if (!tieneDni && !tieneEmail && !tieneGoogle)
            {
                return Results.BadRequest(new
                {
                    success = false,
                    message = "DNI, Email o Google IdToken es requerido"
                });
            }

            // Si es login con contraseña
            if ((tieneDni || tieneEmail) && string.IsNullOrEmpty(request.Contrasena))
            {
                return Results.BadRequest(new
                {
                    success = false,
                    message = "Contraseña es requerida para login con DNI/Email"
                });
            }

            var command = new ClienteLoginCommand(request);
            var result = await mediator.Send(command, cancellationToken);

            if (!result.Success)
            {
                return Results.Unauthorized();
            }

            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new
            {
                success = false,
                message = "Error al autenticar cliente",
                detail = ex.Message
            });
        }
    }

    private static async Task<IResult> GoogleAuth(
        [FromBody] ClienteRegistroDto request,
        IMediator mediator,
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
                    message = "Datos de autenticación inválidos"
                });
            }

            // AutoMapper mapea ClienteRegistroDto a ClienteGoogleAuthCommand
            var command = mapper.Map<ClienteGoogleAuthCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            if (!result.Success)
            {
                return Results.BadRequest(result);
            }

            // Retornar 201 Created si es nuevo cliente, 200 OK si es existente
            if (result.IsNewClient)
            {
                return Results.Created(
                    $"/api/Clientes/{result.Data?.ClienteID}",
                    result);
            }

            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new
            {
                success = false,
                message = "Error al autenticar con Google",
                detail = ex.Message
            });
        }
    }
}
