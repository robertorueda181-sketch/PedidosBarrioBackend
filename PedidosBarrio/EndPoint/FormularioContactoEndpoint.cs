using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Application.Commands.GuardarFormularioContacto;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Api.EndPoint;

public static class FormularioContactoEndpoint
{
    public static void MapFormularioContactoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/Reservar")
                       .WithTags("Reservar")
                       .RequireAuthorization();

        // ===== GUARDAR FORMULARIO DE CONTACTO =====
        group.MapPost("/", async (
            [FromBody] CreateFormularioContactoDto dto,
            IMediator mediator,
            IValidator<CreateFormularioContactoDto> validator) =>
        {
            // Validación con FluentValidation
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var command = new GuardarFormularioContactoCommand(dto);
            var result = await mediator.Send(command);

            return Results.Created($"/api/Reservar/{result.FormularioContactoID}", result);
        })
        .WithName("GuardarFormularioContacto")
        .WithOpenApi()
        .WithSummary("✅ Guardar formulario de contacto")
        .WithDescription("Guarda los datos de un formulario de reservas y se puede asociar a una empresa.");

        // ===== OBTENER FORMULARIO POR ID =====
        group.MapGet("/{id:long}", async (
            long id,
            IMediator mediator) =>
        {
            // Nota: Para obtener un formulario, necesitaríamos crear un query separado.
            // Por simplicidad, devolvemos 501 Not Implemented
            return Results.Problem(
                title: "Funcionalidad no implementada",
                detail: "La obtención de formulario por ID aún no está implementada.",
                statusCode: StatusCodes.Status501NotImplemented);
        })
        .WithName("GetFormularioContactoById")
        .WithOpenApi()
        .WithSummary("🔍 Obtener formulario por ID")
        .WithDescription("Retorna los detalles de un formulario de contacto específico por su ID")
        .Produces<FormularioContactoResponseDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // ===== LISTAR FORMULARIOS (POR EMPRESA) =====
        group.MapGet("/empresa/{empresaId:guid}", async (
            Guid empresaId,
            IMediator mediator) =>
        {
            // Nota: Para listar formularios, necesitaríamos un query separado.
            return Results.Problem(
                title: "Funcionalidad no implementada",
                detail: "La lista de formularios aún no está implementada.",
                statusCode: StatusCodes.Status501NotImplemented);
        })
        .WithName("GetFormulariosByEmpresa")
        .WithOpenApi()
        .WithSummary("📋 Listar formularios por empresa")
        .WithDescription("Retorna todos los formularios de contacto asociados a una empresa")
        .Produces<List<FormularioContactoListDto>>(StatusCodes.Status200OK);
    }
}
