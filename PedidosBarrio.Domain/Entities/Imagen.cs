using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PedidosBarrio.Domain.Entities;

public partial class Imagen
{
    public Imagen() { }

    public Imagen(int? productoID, string urlImagen, Guid empresaID, string descripcion)
    {
        ExternalId = productoID;
        Urlimagen = urlImagen;
            EmpresaID = empresaID;
            Descripcion = descripcion;
            FechaRegistro = DateTime.UtcNow;
            Activa = true;
        }

    [Key]
    [Column("ImagenID")]
    public int ImagenID { get; set; }

    public int? ExternalId { get; set; }

    [Column("URLImagen")]
    [StringLength(500)]
    public string? Urlimagen { get; set; }

    [NotMapped]
    public string? URLImagen { get => Urlimagen; set => Urlimagen = value; }

    public string? Descripcion { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public bool? Activa { get; set; }

    [StringLength(10)]
    public string? Type { get; set; }

    [Column("order")]
    public short Order { get; set; }

    [Column("EmpresaID")]
    public Guid? EmpresaID { get; set; }
}


