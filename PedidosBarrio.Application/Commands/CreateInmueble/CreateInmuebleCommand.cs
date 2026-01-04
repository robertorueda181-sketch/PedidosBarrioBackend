using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.CreateInmueble
{
    public class CreateInmuebleCommand : IRequest<InmuebleDto>
    {
        public Guid EmpresaID { get; set; }
        public int TiposID { get; set; }
        public decimal Precio { get; set; }
        public string Medidas { get; set; }
        public string Ubicacion { get; set; }
        public int Dormitorios { get; set; }
        public int Banos { get; set; }
        public string Descripcion { get; set; }

        public CreateInmuebleCommand(Guid empresaID, int tiposID, decimal precio, string medidas, string ubicacion, int dormitorios, int banos, string descripcion)
        {
            EmpresaID = empresaID;
            TiposID = tiposID;
            Precio = precio;
            Medidas = medidas;
            Ubicacion = ubicacion;
            Dormitorios = dormitorios;
            Banos = banos;
            Descripcion = descripcion;
        }
    }
}
