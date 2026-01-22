namespace PedidosBarrio.Domain.Entities
{
    public class MenuItem
    {
        public short MenuID { get; set; }
        public string Nombre { get; set; }
        public string icon { get; set; }
        public string codigo { get; set; }
        public string padre { get; set; }
        public short order { get; set; }
    }
}
