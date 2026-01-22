using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PedidosBarrio.Infrastructure.Data.Scaffolded.Entities;

[Keyless]
[Table("Menus_TipoEmpresa")]
public partial class MenusTipoEmpresa
{
    public short? TipoEmpresa { get; set; }

    public short? MenuId { get; set; }
}
