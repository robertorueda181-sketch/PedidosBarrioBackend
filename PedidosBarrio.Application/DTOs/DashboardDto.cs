namespace PedidosBarrio.Application.DTOs
{
    public class DashboardDto
    {
        /// <summary>
        /// Cantidad total de productos activos de la empresa
        /// </summary>
        public int CantidadProductos { get; set; }

        /// <summary>
        /// Cantidad de vistas (PageViews) en el día de hoy
        /// </summary>
        public int VistasHoy { get; set; }

        /// <summary>
        /// Información sobre la suscripción activa de la empresa
        /// </summary>
        public SuscripcionDashboardDto? Suscripcion { get; set; }

        /// <summary>
        /// Estadísticas de vistas agrupadas por mes (últimos 12 meses)
        /// </summary>
        public List<EstadisticaMesDto> EstadisticasPorMes { get; set; } = new List<EstadisticaMesDto>();
    }

    public class SuscripcionDashboardDto
    {


        /// <summary>
        /// Nivel/tipo de suscripción (1=Básico, 2=Plus, 3=Premium, etc.)
        /// </summary>
        public short? NivelSuscripcion { get; set; }

        /// <summary>
        /// Fecha de inicio de la suscripción
        /// </summary>
        public DateTime? FechaInicio { get; set; }

        /// <summary>
        /// Fecha de vencimiento de la suscripción
        /// </summary>
        public DateTime? FechaFin { get; set; }

        /// <summary>
        /// Indica si la suscripción está activa
        /// </summary>
        public bool Activa { get; set; }

        /// <summary>
        /// Descripción del nivel (para mostrar: "Básico", "Plus", "Premium")
        /// </summary>
        public string NivelDescripcion { get; set; } = string.Empty;
    }

    public class EstadisticaMesDto
    {
        /// <summary>
        /// Mes en formato "YYYY-MM" (ej: "2024-01")
        /// </summary>
        public string Mes { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del mes en español (ej: "Enero")
        /// </summary>
        public string NombreMes { get; set; } = string.Empty;

        /// <summary>
        /// Año (ej: 2024)
        /// </summary>
        public int Año { get; set; }

        /// <summary>
        /// Cantidad total de vistas en ese mes
        /// </summary>
        public int TotalVistas { get; set; }
    }
}
