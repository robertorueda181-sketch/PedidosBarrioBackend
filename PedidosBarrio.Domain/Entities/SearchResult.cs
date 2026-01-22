namespace PedidosBarrio.Domain.Entities
{
    public class SearchResult
    {
        public string Type { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Location { get; set; }
        public string Category { get; set; }
        public string Url { get; set; }
        public decimal? Price { get; set; }
        public string Operacion { get; set; }
        public string Medidas { get; set; }
        public int? Dormitorios { get; set; }
        public int? Banos { get; set; }
    }
}
