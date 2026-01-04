namespace PedidosBarrio.Application.DTOs
{
    /// <summary>
    /// DTO simple para mostrar imagenes dentro de un Inmueble
    /// Contiene solo la información necesaria para mostrar en el cliente
    /// </summary>
    public class ImagenUrlDto
    {
        public string URLImagen { get; set; }
        public string Descripcion { get; set; }
    }
}
