namespace PedidosBarrio.Domain.Entities
{
    public class Imagen
    {
        public int ImagenID { get; set; }
        public int ProductoID { get; set; }
        public string URLImagen { get; set; }
        public string Descripcion { get; set; }

        public Imagen(int productoID, string urlImagen, string descripcion = null)
        {
            ProductoID = productoID;
            URLImagen = urlImagen;
            Descripcion = descripcion;
        }

        private Imagen() { }
    }
}
