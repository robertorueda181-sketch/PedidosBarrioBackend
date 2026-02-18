using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.CreatePasoInicial;

public class CreatePasoInicialCommand : IRequest<PasoInicialDto>
{
    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    public string Icono { get; set; }
    public string Ruta { get; set; }
    public bool Obligatorio { get; set; } = true;
    public int Orden { get; set; }

    public CreatePasoInicialCommand(
        Guid usuarioId,
        string titulo,
        string descripcion,
        string icono,
        string ruta,
        bool obligatorio = true,
        int orden = 0)
    {
        Titulo = titulo;
        Descripcion = descripcion;
        Icono = icono;
        Ruta = ruta;
        Obligatorio = obligatorio;
        Orden = orden;
    }
}
