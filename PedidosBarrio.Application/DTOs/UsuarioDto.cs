namespace PedidosBarrio.Application.DTOs
{
    public class UsuarioDto
    {
        public Guid UsuarioID { get; set; }
        public string NombreUsuario { get; set; }
        public string Email { get; set; }
        public Guid EmpresaID { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; }
    }
}
