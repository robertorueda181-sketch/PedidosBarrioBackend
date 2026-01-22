using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Infrastructure.Data.Contexts;

public partial class PedidosBarrioDbContext : DbContext
{
    public PedidosBarrioDbContext(DbContextOptions<PedidosBarrioDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<Empresa> Empresas { get; set; }

    public virtual DbSet<EmpresasBaneada> EmpresasBaneadas { get; set; }

    public virtual DbSet<Imagen> Imagenes { get; set; }

    public virtual DbSet<Inmueble> Inmuebles { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<MenusTipoEmpresa> MenusTipoEmpresas { get; set; }

    public virtual DbSet<Negocio> Negocios { get; set; }

    public virtual DbSet<Precio> Precios { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Suscripcion> Suscripciones { get; set; }

    public virtual DbSet<Tipo> Tipos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("pgcrypto")
                    .HasPostgresExtension("uuid-ossp");

        // Aplicar configuraciones externas
        modelBuilder.ApplyConfiguration(new PedidosBarrio.Infrastructure.Data.EntityConfigurations.ProductoConfiguration());
        modelBuilder.ApplyConfiguration(new PedidosBarrio.Infrastructure.Data.EntityConfigurations.CategoriaConfiguration());
        modelBuilder.ApplyConfiguration(new PedidosBarrio.Infrastructure.Data.EntityConfigurations.PrecioConfiguration());
        modelBuilder.ApplyConfiguration(new PedidosBarrio.Infrastructure.Data.EntityConfigurations.ConfiguracionConfiguration());
        modelBuilder.ApplyConfiguration(new PedidosBarrio.Infrastructure.Data.EntityConfigurations.ImagenConfiguration());

        // Configuraciones de otras entidades se manejan por Data Annotations en el Dominio

        // Configuraciones globales para PostgreSQL (lowercase names)
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Set table name to public default or entity name
            // Use quotes for PostgreSQL case sensitivity if needed
        }
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}


