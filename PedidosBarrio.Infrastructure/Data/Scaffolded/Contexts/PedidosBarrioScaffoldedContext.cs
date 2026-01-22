using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Infrastructure.Data.Scaffolded.Entities;

namespace PedidosBarrio.Infrastructure.Data.Scaffolded.Contexts;

public partial class PedidosBarrioScaffoldedContext : DbContext
{
    public PedidosBarrioScaffoldedContext(DbContextOptions<PedidosBarrioScaffoldedContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<Empresa> Empresas { get; set; }

    public virtual DbSet<EmpresasBaneada> EmpresasBaneadas { get; set; }

    public virtual DbSet<Imagene> Imagenes { get; set; }

    public virtual DbSet<Inmueble> Inmuebles { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<MenusTipoEmpresa> MenusTipoEmpresas { get; set; }

    public virtual DbSet<Negocio> Negocios { get; set; }

    public virtual DbSet<Precio> Precios { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Suscripcione> Suscripciones { get; set; }

    public virtual DbSet<Tipo> Tipos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresExtension("pgcrypto")
            .HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.CategoriaId).HasName("Categorias_pkey");

            entity.Property(e => e.Activo).HasDefaultValue(true);
        });

        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.HasKey(e => e.EmpresaId).HasName("Empresas_pkey");

            entity.Property(e => e.EmpresaId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Activa).HasDefaultValue(true);
            entity.Property(e => e.Aprobado).HasDefaultValue(false);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.TipoEmpresa).HasDefaultValue((short)1);
            entity.Property(e => e.Visible).HasDefaultValue(true);
        });

        modelBuilder.Entity<Imagene>(entity =>
        {
            entity.HasKey(e => e.ImagenId).HasName("Imagenes_pkey");

            entity.Property(e => e.ImagenId).UseIdentityAlwaysColumn();
            entity.Property(e => e.Activa).HasDefaultValue(true);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Order).HasDefaultValue((short)1);
        });

        modelBuilder.Entity<Inmueble>(entity =>
        {
            entity.HasKey(e => e.InmuebleId).HasName("Inmuebles_pkey");

            entity.Property(e => e.InmuebleId).UseIdentityAlwaysColumn();
            entity.Property(e => e.Activa).HasDefaultValue(true);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Empresa).WithMany(p => p.Inmuebles)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Inmuebles_EmpresaID_fkey");

            entity.HasOne(d => d.Tipos).WithMany(p => p.Inmuebles).HasConstraintName("Inmuebles_TiposID_fkey");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("Logs_pkey");

            entity.Property(e => e.Category).HasDefaultValueSql("'General'::character varying");
            entity.Property(e => e.Timestamp).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.MenuId).HasName("Menus_pkey");

            entity.Property(e => e.MenuId).UseIdentityAlwaysColumn();
        });

        modelBuilder.Entity<Negocio>(entity =>
        {
            entity.HasKey(e => e.NegocioId).HasName("Negocios_pkey");

            entity.Property(e => e.NegocioId).UseIdentityAlwaysColumn();
            entity.Property(e => e.Activa).HasDefaultValue(true);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Empresa).WithMany(p => p.Negocios)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Negocios_EmpresaID_fkey");

            entity.HasOne(d => d.Tipos).WithMany(p => p.Negocios).HasConstraintName("Negocios_TiposID_fkey");
        });

        modelBuilder.Entity<Precio>(entity =>
        {
            entity.HasKey(e => e.IdPrecio).HasName("Precios_pkey");

            entity.Property(e => e.IdPrecio).UseIdentityAlwaysColumn();
            entity.Property(e => e.Principal).HasDefaultValue(false);

            entity.HasOne(d => d.External).WithMany(p => p.Precios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PRECIOS_PRODUCTO");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.ProductoId).HasName("Productos_pkey");

            entity.Property(e => e.ProductoId).UseIdentityAlwaysColumn();
            entity.Property(e => e.Activa).HasDefaultValue(true);
            entity.Property(e => e.Aprobado).HasDefaultValue(true);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Inventario).HasDefaultValue(false);
            entity.Property(e => e.Stock).HasDefaultValue(0);
            entity.Property(e => e.Visible).HasDefaultValue(true);

            entity.HasOne(d => d.Empresa).WithMany(p => p.Productos)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Productos_EmpresaID_fkey");
        });

        modelBuilder.Entity<Suscripcione>(entity =>
        {
            entity.HasKey(e => e.SuscripcionId).HasName("Suscripciones_pkey");

            entity.Property(e => e.SuscripcionId).UseIdentityAlwaysColumn();
            entity.Property(e => e.Activa).HasDefaultValue(true);
            entity.Property(e => e.FechaInicio).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Empresa).WithMany(p => p.Suscripciones)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Suscripciones_EmpresaID_fkey");
        });

        modelBuilder.Entity<Tipo>(entity =>
        {
            entity.HasKey(e => e.TipoId).HasName("Tipos_pkey");

            entity.Property(e => e.TipoId).UseIdentityAlwaysColumn();
            entity.Property(e => e.Activa).HasDefaultValue(true);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("Usuarios_pkey");

            entity.Property(e => e.UsuarioId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Activa).HasDefaultValue(true);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
