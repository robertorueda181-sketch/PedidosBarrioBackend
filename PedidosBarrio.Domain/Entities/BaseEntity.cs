namespace PedidosBarrio.Domain.Entities
{
    public class BaseEntity
    {
        public Guid ID { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public bool Activa { get; set; }
    }
}

