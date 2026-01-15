using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.CreateCategoria
{
    public class CreateCategoriaCommand : IRequest<CategoriaDto>
    {
        public string Descripcion { get; set; }
        public string Color { get; set; }

        public CreateCategoriaCommand(string descripcion, string color)
        {
            Descripcion = descripcion;
            Color = color;
        }
    }
}