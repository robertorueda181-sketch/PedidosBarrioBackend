namespace PedidosBarrio.Domain.Entities
{
    public class Suscripcion
    {
        public int SuscripcionID { get; set; }
        public int EmpresaID { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool Activa { get; set; }
        public decimal Monto { get; set; }

        public Suscripcion(int empresaID, decimal monto, DateTime? fechaFin = null)
        {
            EmpresaID = empresaID;
            Monto = monto;
            FechaInicio = DateTime.UtcNow;
            FechaFin = fechaFin;
            Activa = true;
        }

        private Suscripcion() { }
    }
}
