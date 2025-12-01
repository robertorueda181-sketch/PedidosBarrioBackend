namespace PedidosBarrio.Application.DTOs
{
    public class NegocioDto
    {
        public int NegocioID { get; set; }
        public int EmpresaID { get; set; }
        public int TiposID { get; set; }
        public string URLNegocio { get; set; }
        public string URLOpcional { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
