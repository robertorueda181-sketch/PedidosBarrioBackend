namespace PedidosBarrio.Application.DTOs
{
    public class CreateSuscripcionDto
    {
        public int EmpresaID { get; set; }
        public decimal Monto { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
