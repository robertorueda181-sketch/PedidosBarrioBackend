namespace PedidosBarrio.Application.DTOs
{
    public class CreateUsuarioDto
    {
        public string Email { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public Guid EmpresaID { get; set; }
    }
}
