namespace PedidosBarrio.Application.DTOs
{
    public class CreateUsuarioDto
    {
        public string NombreUsuario { get; set; }
        public string Email { get; set; }
        public string Contrasena { get; set; }
        public Guid EmpresaID { get; set; }
    }
}
