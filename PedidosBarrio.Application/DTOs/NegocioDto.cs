namespace PedidosBarrio.Application.DTOs
{
    public class NegocioDto
    {
        public int NegocioID { get; set; }
        public Guid EmpresaID { get; set; }
        public int TiposID { get; set; }
        public string URLNegocio { get; set; }
        public string Descripcion { get; set; }
        public string UrlImagen { get; set; }
    }
}
