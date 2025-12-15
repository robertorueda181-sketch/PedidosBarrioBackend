namespace PedidosBarrio.Application.DTOs
{
    public class LoginResponseDto
    {
        public Guid UsuarioID { get; set; }
        public string NombreUsuario { get; set; }
        public string Email { get; set; }
        public Guid EmpresaID { get; set; }
        public string Mensaje { get; set; }
    }
}
