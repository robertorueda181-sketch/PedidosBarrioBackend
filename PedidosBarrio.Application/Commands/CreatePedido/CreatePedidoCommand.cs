using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.CreatePedido;

public record CreatePedidoCommand(CreatePedidoDto Data) : IRequest<CreatePedidoResponse>;

public record CreatePedidoResponse(long PedidoID, Guid PedidoUID, decimal Total);
