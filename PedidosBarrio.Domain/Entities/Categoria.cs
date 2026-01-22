using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PedidosBarrio.Domain.Entities;

public partial class Categoria
{
    public Categoria() { }

    public Categoria(Guid empresaId, string descripcion, string color)
    {
        EmpresaID = empresaId;
        Descripcion = descripcion;
        Color = color;
        Activa = true;
    }

    public Categoria(short id, Guid empresaId, string descripcion, string color, bool? activa)
    {
        CategoriaID = id;
        EmpresaID = empresaId;
        Descripcion = descripcion;
        Color = color;
        Activa = activa;
    }

    [Key]
    [Column("CategoriaID")]
    public short CategoriaID { get; set; }

    [NotMapped]
    public bool Activo { get => Activa ?? false; set => Activa = value; }

    [Column("EmpresaID")]
    public Guid EmpresaID { get; set; }

    [StringLength(50)]
    public string Descripcion { get; set; } = null!;

    public bool? Activa { get; set; }

    [StringLength(15)]
    public string? Color { get; set; }
}


