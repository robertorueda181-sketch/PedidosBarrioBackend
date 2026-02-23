using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.ClienteAuth;

public record ClienteLoginCommand(ClienteLoginDto Data) : IRequest<ClienteAuthResponseDto>;
