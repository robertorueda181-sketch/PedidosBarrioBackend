using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.UpdateSuscripcion
{
    public class UpdateSuscripcionCommand : IRequest<SuscripcionDto>
    {
        public int SuscripcionID { get; set; }
        public int EmpresaID { get; set; }
        public decimal Monto { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool Activa { get; set; }

        public UpdateSuscripcionCommand(int suscripcionID, int empresaID, decimal monto, DateTime? fechaFin, bool activa)
        {
            SuscripcionID = suscripcionID;
            EmpresaID = empresaID;
            Monto = monto;
            FechaFin = fechaFin;
            Activa = activa;
        }
    }
}
