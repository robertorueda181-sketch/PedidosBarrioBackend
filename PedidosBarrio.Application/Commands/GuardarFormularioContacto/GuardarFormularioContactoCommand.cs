using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.GuardarFormularioContacto;

public record GuardarFormularioContactoCommand(CreateFormularioContactoDto Data) : IRequest<FormularioContactoResponseDto>;
