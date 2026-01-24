namespace PedidosBarrio.Application.DTOs
{
    public class LoginResponseDto
    {
        public Guid UsuarioID { get; set; }
        public string Email { get; set; }
        public string NombreCompleto { get; set; }
        public Guid EmpresaID { get; set; }
        public string NombreEmpresa { get; set; }
        public string TipoEmpresa { get; set; }
        public string Categoria { get; set; }
        public string Telefono { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime TokenExpiracion { get; set; }
        public bool EsNuevo { get; set; }
        public string Mensaje { get; set; }
    }
}
