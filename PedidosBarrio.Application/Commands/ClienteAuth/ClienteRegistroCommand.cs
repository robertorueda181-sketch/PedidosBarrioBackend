using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.ClienteAuth;

public record ClienteRegistroCommand(ClienteRegistroDto Data) : IRequest<ClienteAuthResponseDto>;
