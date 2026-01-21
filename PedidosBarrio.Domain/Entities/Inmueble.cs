namespace PedidosBarrio.Domain.Entities
{
    public class Inmueble
    {
        public int InmuebleID { get; set; }
        public Guid EmpresaID { get; set; }
        public int TiposID { get; set; }
        public string Tipo{ get; set; }
        public int? OperacionID { get; set; }
        public decimal Precio { get; set; }
        public string Medidas { get; set; }
        public string Ubicacion { get; set; }
        public int Dormitorios { get; set; }
        public int Banos { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Activa { get; set; }

        public Imagen Imagen { get; set; } = new Imagen();
        public Tipo Operacion { get; set; } = new();
        public string Latitud { get; set; }
        public string Longitud { get; set; } 

        public Inmueble(Guid empresaID, int tiposID, decimal precio, string medidas, string ubicacion, int dormitorios, int banos, string descripcion, int? operacionID = null)
        {
            EmpresaID = empresaID;
            TiposID = tiposID;
            OperacionID = operacionID;
            Precio = precio;
            Medidas = medidas;
            Ubicacion = ubicacion;
            Dormitorios = dormitorios;
            Banos = banos;
            Descripcion = descripcion;
            FechaRegistro = DateTime.UtcNow;
            Activa = true;
        }

        private Inmueble() { }
    }
}
