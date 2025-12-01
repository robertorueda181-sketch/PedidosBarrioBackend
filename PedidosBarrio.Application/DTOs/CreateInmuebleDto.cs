namespace PedidosBarrio.Application.DTOs
{
    public class CreateInmuebleDto
    {
        public int EmpresaID { get; set; }
        public int TiposID { get; set; }
        public decimal Precio { get; set; }
        public string Medidas { get; set; }
        public string Ubicacion { get; set; }
        public int Dormitorios { get; set; }
        public int Banos { get; set; }
        public string Descripcion { get; set; }
    }
}
