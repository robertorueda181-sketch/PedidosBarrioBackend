namespace PedidosBarrio.Application.DTOs
{
    public class CreateSuscripcionDto
    {
        public Guid EmpresaID { get; set; }
        public decimal Monto { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
