namespace PedidosBarrio.Domain.Entities
{
    public class Imagen
    {
        public int ImagenID { get; set; }
        public int ExternalId { get; set; }
        public string URLImagen { get; set; }
        public string Descripcion { get; set; }
        public string Type { get; set; }
        public bool Activa { get; set; }



        public Imagen(int externalId, string urlImagen, string descripcion = null, string type = "prod")
        {
            ExternalId = externalId;
            URLImagen = urlImagen;
            Descripcion = descripcion;
            Type = type;
            Activa = true;
        }

        public Imagen() { }
    }
}
