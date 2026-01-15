using MediatR;

namespace PedidosBarrio.Application.Commands.DeleteCategoria
{
    public class DeleteCategoriaCommand : IRequest
    {
        public short CategoriaId { get; set; }

        public DeleteCategoriaCommand(short categoriaId)
        {
            CategoriaId = categoriaId;
        }
    }
}