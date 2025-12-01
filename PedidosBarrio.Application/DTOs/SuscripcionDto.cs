namespace PedidosBarrio.Application.DTOs
{
    public class SuscripcionDto
    {
        public int SuscripcionID { get; set; }
        public int EmpresaID { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool Activa { get; set; }
        public decimal Monto { get; set; }
    }
}
