using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedidosBarrio.Domain.Entities;

/// <summary>
/// Entidad para almacenar páginas web dinámicas por empresa
/// Permite guardar contenido JSONB para cada página identificada por un código único
/// </summary>
[Table("Paginas")]
public class Pagina
{
    public Pagina() { }

    public Pagina(Guid codigoEmpresa, string contenido)
    {
        PaginaID = Guid.NewGuid();
        CodigoEmpresa = codigoEmpresa;
        Contenido = contenido;
        var now = DateTime.UtcNow;
        FechaCreacion = DateTime.SpecifyKind(now, DateTimeKind.Utc);
        FechaActualizacion = DateTime.SpecifyKind(now, DateTimeKind.Utc);
    }

    [Key]
    [Column("PaginaID")]
    public Guid PaginaID { get; set; }

    /// <summary>
    /// Código único de la empresa (ej: 'restaurante-01', 'tienda-02')
    /// </summary>
    [Column("CodigoEmpresa")]
    public Guid CodigoEmpresa { get; set; }



    /// <summary>
    /// Contenido JSONB de la página
    /// Puede almacenar cualquier estructura JSON (banners, secciones, textos, imágenes, etc.)
    /// </summary>
    [Column("Contenido", TypeName = "jsonb")]
    public string Contenido { get; set; } = null!;

    /// <summary>
    /// Descripción o título de la página (opcional)
    /// </summary>
    [Column("Descripcion")]
    [StringLength(500)]
    public string? Descripcion { get; set; }

    /// <summary>
    /// Indica si la página está activa/visible
    /// </summary>
    [Column("Activa")]
    public bool Activa { get; set; } = true;

    /// <summary>
    /// Fecha de creación del registro
    /// </summary>
    [Column("FechaCreacion")]
    public DateTime FechaCreacion { get; set; }

    /// <summary>
    /// Última fecha de actualización del registro
    /// </summary>
    [Column("FechaActualizacion")]
    public DateTime FechaActualizacion { get; set; }

    public string TemplateBase { get; set; } = "";
}
