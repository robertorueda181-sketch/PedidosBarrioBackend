namespace PedidosBarrio.Application.DTOs
{
    public class EmpresaSedeDetalleDto
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string? Email { get; set; }
        public string? LogoUrl { get; set; }
        public string? ProfileImageUrl { get; set; }

        // Redes Sociales
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public string? Twitter { get; set; }
        public string? Tiktok { get; set; }
        public string? Whatsapp { get; set; }

        // Telfonos
        public string? TelefonoPrincipal { get; set; }
        public string? TelefonoSec { get; set; }

        // Direccin (Sede)
        public int? DireccionID { get; set; }
        public string? NombreLocal { get; set; }
        public string? Direccion { get; set; }
        public decimal Longitud { get; set; }
        public decimal Latitud { get; set; }
        public string? Departamento { get; set; }
        public string? Provincia { get; set; }
        public string? Distrito { get; set; }
        public string? Referencia { get; set; }
    }

    public class SaveEmpresaSedeDto
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string? Telefono { get; set; }
        public string? Telefono2 { get; set; }
        public string? Correo { get; set; }
        public string? UrlLogo { get; set; }
        public SaveRedesSocialesDto RedesSociales { get; set; }
        public SaveDireccionSedeDto Direccion { get; set; }
    }

    public class SaveRedesSocialesDto
    {
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public string? Twitter { get; set; }
        public string? Tiktok { get; set; }
        public string? Whatsapp { get; set; }
    }

    public class SaveDireccionSedeDto
    {
        public string DireccionCompleta { get; set; }
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public string? Departamento { get; set; }
        public string? Provincia { get; set; }
        public string? Distrito { get; set; }
        public string? NombreLocal { get; set; }
        public string? Referencia { get; set; }
    }
}
