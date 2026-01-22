namespace PedidosBarrio.Domain.Entities
{
    public class Configuracion
    {
        public int ConfiguracionID { get; set; }
        public Guid EmpresaID { get; set; }
        public string Clave { get; set; }
        public string Valor { get; set; }
        public DateTime FechaModificacion { get; set; }
        public bool Activa { get; set; }

        public Configuracion(Guid empresaId, string clave, string valor)
        {
            EmpresaID = empresaId;
            Clave = clave;
            Valor = valor;
            FechaModificacion = DateTime.UtcNow;
            Activa = true;
        }

        private Configuracion() { }
    }
}
