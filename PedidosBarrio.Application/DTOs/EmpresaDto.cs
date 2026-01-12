namespace PedidosBarrio.Application.DTOs
{
    public class EmpresaDto
    {
        public Guid UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Direccion { get; set; }
        public string Referencia { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Activa { get; set; }
    }
}
