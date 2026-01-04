namespace PedidosBarrio.Application.DTOs
{
    public class InmuebleDto
    {
        public int InmuebleID { get; set; }
        public Guid EmpresaID { get; set; }
        public int TiposID { get; set; }
        public string Tipo { get; set; }
        public decimal Precio { get; set; }
        public string Medidas { get; set; }
        public string Ubicacion { get; set; }
        public int Dormitorios { get; set; }
        public int Banos { get; set; }
        public string Descripcion { get; set; }

        public string URLImagen { get; set; }
        public string Operacion { get; set; }

    }
}
