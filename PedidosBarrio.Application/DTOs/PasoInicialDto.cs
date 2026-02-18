namespace PedidosBarrio.Application.DTOs;

public class PasoInicialDto
{
    public int PasoID { get; set; }
    public Guid EmpresaID { get; set; }
    public string Codigo { get; set; }
    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    public string Icono { get; set; }
    public string Ruta { get; set; }
    public bool Obligatorio { get; set; }
    public bool Completado { get; set; }
    public int Orden { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaCompletado { get; set; }
}

public class ActualizarPasoInicialDto
{
    public bool Completado { get; set; }
}

public class CrearPasosInicialesDto
{
    public Guid EmpresaID { get; set; }
}

public class PasosPendientesDto
{
    public bool TienePasosPendientes { get; set; }
    public int TotalPasos { get; set; }
    public int PasosCompletados { get; set; }
    public int PasosPendientes { get; set; }
}
