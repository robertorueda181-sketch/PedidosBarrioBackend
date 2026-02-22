using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetDashboard
{
    public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, DashboardDto>
    {
        private readonly IProductoRepository _productoRepository;
        private readonly IPageViewRepository _pageViewRepository;
        private readonly ISuscripcionRepository _suscripcionRepository;
        private readonly IEmpresaRepository _empresaRepository;

        public GetDashboardQueryHandler(
            IProductoRepository productoRepository,
            IPageViewRepository pageViewRepository,
            ISuscripcionRepository suscripcionRepository,
            IEmpresaRepository empresaRepository)
        {
            _productoRepository = productoRepository;
            _pageViewRepository = pageViewRepository;
            _suscripcionRepository = suscripcionRepository;
            _empresaRepository = empresaRepository;
        }

        public async Task<DashboardDto> Handle(GetDashboardQuery query, CancellationToken cancellationToken)
        {
            var empresaId = query.EmpresaID;
            var dashboard = new DashboardDto();

            // Obtener información de la empresa y su zona horaria
            var empresa = await _empresaRepository.GetByIdAsync(empresaId);
            var timeZoneId = empresa?.TimeZoneId ?? "UTC";
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var ahora = TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZone);

            // 1. Obtener cantidad de productos activos
            var productos = await _productoRepository.GetByEmpresaIdAsync(empresaId);
            dashboard.CantidadProductos = productos.Count(p => p.Activa == true && p.Aprobado == true);

            // 2. Obtener vistas del día de hoy (usando zona horaria de la empresa)
            var hoyLocal = ahora.Date;
            var mananaLocal = hoyLocal.AddDays(1);

            // Convertir las fechas locales de vuelta a UTC para la consulta a BD
            var hoyUtc = TimeZoneInfo.ConvertTimeToUtc(hoyLocal, timeZone);
            var mananaUtc = TimeZoneInfo.ConvertTimeToUtc(mananaLocal, timeZone);

            var vistasHoy = await _pageViewRepository.GetByEmpresaAndDateRangeAsync(empresaId, hoyUtc, mananaUtc);
            dashboard.VistasHoy = vistasHoy.Count();

            // 3. Obtener información de suscripción activa
            var suscripciones = await _suscripcionRepository.GetByEmpresaIdAsync(empresaId);
            var suscripcion = suscripciones
                .Where(s => s.Activa == true)
                .OrderByDescending(s => s.FechaInicio)
                .FirstOrDefault();

            if (suscripcion != null)
            {
                dashboard.Suscripcion = new SuscripcionDashboardDto
                {
                    NivelSuscripcion = suscripcion.NivelSuscripcion,
                    FechaInicio = suscripcion.FechaInicio,
                    FechaFin = suscripcion.FechaFin,
                    Activa = suscripcion.Activa ?? false,
                    NivelDescripcion = GetNivelDescripcion(suscripcion.NivelSuscripcion)
                };
            }

            // 4. Obtener estadísticas de vistas por mes (últimos 12 meses)
            var hace12MesesLocal = ahora.AddMonths(-12).Date;
            var hoyLocalEnd = ahora.Date.AddDays(1);

            // Convertir a UTC para la consulta
            var hace12MesesUtc = TimeZoneInfo.ConvertTimeToUtc(hace12MesesLocal, timeZone);
            var hoyUtcEnd = TimeZoneInfo.ConvertTimeToUtc(hoyLocalEnd, timeZone);

            var vistasUltimo12Meses = await _pageViewRepository.GetByEmpresaAndDateRangeAsync(
                empresaId,
                hace12MesesUtc,
                hoyUtcEnd
            );

            var estadisticasPorMes = vistasUltimo12Meses
                .GroupBy(pv => new { Año = pv.Fecha.Year, Mes = pv.Fecha.Month })
                .OrderBy(g => g.Key.Año)
                .ThenBy(g => g.Key.Mes)
                .Select(g => new EstadisticaMesDto
                {
                    Año = g.Key.Año,
                    Mes = $"{g.Key.Año:0000}-{g.Key.Mes:00}",
                    NombreMes = GetNombreMes(g.Key.Mes),
                    TotalVistas = g.Count()
                })
                .ToList();

            // Asegurar que todos los meses de los últimos 12 meses aparezcan (incluso con 0 vistas)
            var mesesEsperados = GenerarMesesUltimos12Meses(ahora);
            foreach (var mes in mesesEsperados)
            {
                if (!estadisticasPorMes.Any(e => e.Mes == mes.Mes))
                {
                    estadisticasPorMes.Add(mes);
                }
            }

            dashboard.EstadisticasPorMes = estadisticasPorMes.OrderBy(e => e.Mes).ToList();

            return dashboard;
        }

        /// <summary>
        /// Obtiene la descripción del nivel de suscripción
        /// </summary>
        private string GetNivelDescripcion(short? nivel)
        {
            return nivel switch
            {
                1 => "Básico",
                2 => "Plus",
                3 => "Premium",
                4 => "Enterprise",
                _ => "Desconocido"
            };
        }

        /// <summary>
        /// Obtiene el nombre del mes en español
        /// </summary>
        private string GetNombreMes(int mes)
        {
            return mes switch
            {
                1 => "Enero",
                2 => "Febrero",
                3 => "Marzo",
                4 => "Abril",
                5 => "Mayo",
                6 => "Junio",
                7 => "Julio",
                8 => "Agosto",
                9 => "Septiembre",
                10 => "Octubre",
                11 => "Noviembre",
                12 => "Diciembre",
                _ => "Desconocido"
            };
        }

        /// <summary>
        /// Genera lista de meses para los últimos 12 meses
        /// </summary>
        private List<EstadisticaMesDto> GenerarMesesUltimos12Meses(DateTime ahora)
        {
            var meses = new List<EstadisticaMesDto>();

            for (int i = 11; i >= 0; i--)
            {
                var fecha = ahora.AddMonths(-i);
                meses.Add(new EstadisticaMesDto
                {
                    Año = fecha.Year,
                    Mes = $"{fecha.Year:0000}-{fecha.Month:00}",
                    NombreMes = GetNombreMes(fecha.Month),
                    TotalVistas = 0
                });
            }

            return meses;
        }
    }
}
