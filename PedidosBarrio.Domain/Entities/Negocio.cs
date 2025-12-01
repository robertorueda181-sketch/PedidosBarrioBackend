namespace PedidosBarrio.Domain.Entities
{
    public class Negocio
    {
        public int NegocioID { get; set; }
        public int EmpresaID { get; set; }
        public int TiposID { get; set; }
        public string URLNegocio { get; set; }
        public string URLOpcional { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaRegistro { get; set; }

        public Negocio(int empresaID, int tiposID, string urlNegocio, string descripcion, string urlOpcional = null)
        {
            EmpresaID = empresaID;
            TiposID = tiposID;
            URLNegocio = urlNegocio;
            URLOpcional = urlOpcional;
            Descripcion = descripcion;
            FechaRegistro = DateTime.UtcNow;
        }

        private Negocio() { }
    }
}
