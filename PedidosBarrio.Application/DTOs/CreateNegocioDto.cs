namespace PedidosBarrio.Application.DTOs
{
    public class CreateNegocioDto
    {
        public Guid EmpresaID { get; set; }
        public int TiposID { get; set; }
        public string URLNegocio { get; set; }
        public string Descripcion { get; set; }
        public string URLOpcional { get; set; }
    }
}
