using MediatR;

namespace PedidosBarrio.Application.Commands.UpdateCategoria
{
    public class UpdateCategoriaCommand : IRequest
    {
        public short CategoriaId { get; set; }
        public string Descripcion { get; set; }
        public string Color { get; set; }

        public UpdateCategoriaCommand(short categoriaId, string descripcion, string color)
        {
            CategoriaId = categoriaId;
            Descripcion = descripcion;
            Color = color;
        }
    }
}