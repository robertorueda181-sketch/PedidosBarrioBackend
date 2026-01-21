namespace PedidosBarrio.Domain.Entities
{
    public class Imagen
    {
        public int ImagenID { get; set; }
        public int ExternalId { get; set; } // ProductoID
        public string URLImagen { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Activa { get; set; }
        public string Type { get; set; }
        public short Order { get; set; }
        public Guid EmpresaID { get; set; }

        // Navigation property
        public Producto Producto { get; set; }

        public Imagen(int externalId, string urlImagen, Guid empresaId, string descripcion = "", string type = "PRODUCT")
        {
            ExternalId = externalId;
            URLImagen = urlImagen;
            EmpresaID = empresaId;
            Descripcion = descripcion;
            Type = type;
            Order = 1;
            FechaRegistro = DateTime.UtcNow;
            Activa = true;
        }

        public Imagen() { }
    }
}
