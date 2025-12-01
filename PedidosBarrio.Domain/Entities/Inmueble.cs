namespace PedidosBarrio.Domain.Entities
{
    public class Inmueble
    {
        public int InmuebleID { get; set; }
        public int EmpresaID { get; set; }
        public int TiposID { get; set; }
        public decimal Precio { get; set; }
        public string Medidas { get; set; }
        public string Ubicacion { get; set; }
        public int Dormitorios { get; set; }
        public int Banos { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaRegistro { get; set; }

        public Inmueble(int empresaID, int tiposID, decimal precio, string medidas, string ubicacion, int dormitorios, int banos, string descripcion)
        {
            EmpresaID = empresaID;
            TiposID = tiposID;
            Precio = precio;
            Medidas = medidas;
            Ubicacion = ubicacion;
            Dormitorios = dormitorios;
            Banos = banos;
            Descripcion = descripcion;
            FechaRegistro = DateTime.UtcNow;
        }

        private Inmueble() { }
    }
}
