using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.CreateSuscripcion
{
    public class CreateSuscripcionCommand : IRequest<SuscripcionDto>
    {
        public int EmpresaID { get; set; }
        public decimal Monto { get; set; }
        public DateTime? FechaFin { get; set; }

        public CreateSuscripcionCommand(int empresaID, decimal monto, DateTime? fechaFin = null)
        {
            EmpresaID = empresaID;
            Monto = monto;
            FechaFin = fechaFin;
        }
    }
}
