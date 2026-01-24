namespace PedidosBarrio.Application.DTOs
{
    public class EnviarCodigoVerifDto
    {
        public string Correo { get; set; }
    }

    public class VerificarCodigoDto
    {
        public string Correo { get; set; }
        public string Codigo { get; set; }
    }
}
