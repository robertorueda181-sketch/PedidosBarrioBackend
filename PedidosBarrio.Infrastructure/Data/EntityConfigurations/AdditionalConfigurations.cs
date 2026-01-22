using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Infrastructure.Data.EntityConfigurations
{
    // =============================================
    // CONFIGURACIONES ENTITY FRAMEWORK CORE
    // =============================================

    public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
    {
        public void Configure(EntityTypeBuilder<Producto> builder)
        {
            builder.ToTable("Productos");
            builder.HasKey(p => p.ProductoID);
            
            builder.Property(p => p.ProductoID).HasColumnName("ProductoID").ValueGeneratedOnAdd();
            builder.Property(p => p.EmpresaID).HasColumnName("EmpresaID").IsRequired();
            builder.Property(p => p.CategoriaID).HasColumnName("CategoriaID").IsRequired();
            builder.Property(p => p.Nombre).HasColumnName("Nombre").HasMaxLength(200).IsRequired();
            builder.Property(p => p.Descripcion).HasColumnName("Descripcion").HasMaxLength(1000);
            builder.Property(p => p.FechaRegistro).HasColumnName("FechaRegistro").HasColumnType("timestamp with time zone");
            builder.Property(p => p.Stock).HasColumnName("Stock").HasDefaultValue(0);
            builder.Property(p => p.StockMinimo).HasColumnName("StockMinimo");
            builder.Property(p => p.Activa).HasColumnName("Activa").HasDefaultValue(true);
            builder.Property(p => p.Visible).HasColumnName("Visible").HasDefaultValue(true);
            builder.Property(p => p.Inventario).HasColumnName("Inventario").HasDefaultValue(false);
            
            builder.HasIndex(p => p.EmpresaID).HasDatabaseName("IX_Productos_EmpresaID");
            builder.HasIndex(p => p.CategoriaID).HasDatabaseName("IX_Productos_CategoriaID");
        }
    }

    public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
    {
        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            builder.ToTable("Categorias");
            builder.HasKey(c => c.CategoriaID);
            
            builder.Property(c => c.CategoriaID).HasColumnName("CategoriaID").ValueGeneratedOnAdd();
            builder.Property(c => c.EmpresaID).HasColumnName("EmpresaID").IsRequired();
            builder.Property(c => c.Descripcion).HasColumnName("Descripcion").HasMaxLength(200).IsRequired();
            builder.Property(c => c.Color).HasColumnName("Color").HasMaxLength(7).HasDefaultValue("#007bff");
            
            builder.HasIndex(c => c.EmpresaID).HasDatabaseName("IX_Categorias_EmpresaID");
        }
    }

    public class PrecioConfiguration : IEntityTypeConfiguration<Precio>
    {
        public void Configure(EntityTypeBuilder<Precio> builder)
        {
            builder.ToTable("Precios");
            builder.HasKey(p => p.IdPrecio);
            
            builder.Property(p => p.IdPrecio).HasColumnName("IdPrecio").ValueGeneratedOnAdd();
            builder.Property(p => p.PrecioValor).HasColumnName("Precio").HasColumnType("decimal(12,2)").IsRequired();
            builder.Property(p => p.ExternalId).HasColumnName("ExternalId").IsRequired();
            builder.Property(p => p.EmpresaID).HasColumnName("EmpresaID").IsRequired();
            builder.Property(p => p.Principal).HasColumnName("EsPrincipal").HasDefaultValue(false);

            builder.HasIndex(p => p.ExternalId).HasDatabaseName("IX_Precios_ExternalId");
            builder.HasIndex(p => p.EmpresaID).HasDatabaseName("IX_Precios_EmpresaID");
        }
    }

    public class ConfiguracionConfiguration : IEntityTypeConfiguration<Configuracion>
    {
        public void Configure(EntityTypeBuilder<Configuracion> builder)
        {
            builder.ToTable("Configuraciones");
            builder.HasKey(c => c.ConfiguracionID);
            builder.Property(c => c.ConfiguracionID).HasColumnName("ConfiguracionID").ValueGeneratedOnAdd();
            builder.Property(c => c.EmpresaID).HasColumnName("EmpresaID").IsRequired();
            builder.Property(c => c.Clave).HasColumnName("Clave").HasMaxLength(100).IsRequired();
            builder.Property(c => c.Valor).HasColumnName("Valor").HasMaxLength(1000);
            builder.Property(c => c.FechaModificacion).HasColumnName("FechaModificacion").HasColumnType("timestamp with time zone");
            builder.Property(c => c.Activa).HasColumnName("Activa").HasDefaultValue(true);
            builder.HasIndex(c => new { c.EmpresaID, c.Clave }).IsUnique();
        }
    }

    public class ImagenConfiguration : IEntityTypeConfiguration<Imagen>
    {
        public void Configure(EntityTypeBuilder<Imagen> builder)
        {
            builder.ToTable("Imagen");
            builder.HasKey(i => i.ImagenID);

            builder.Property(i => i.ImagenID)
                   .HasColumnName("ImagenID")
                   .UseIdentityAlwaysColumn();

            builder.Property(i => i.ExternalId).HasColumnName("ExternalId");
            builder.Property(i => i.Urlimagen).HasColumnName("URLImagen").HasMaxLength(500);
            builder.Property(i => i.Descripcion).HasColumnName("Descripcion");
            builder.Property(i => i.EmpresaID).HasColumnName("EmpresaID");
            builder.Property(i => i.Type).HasColumnName("Type").HasMaxLength(10);
            builder.Property(i => i.Order).HasColumnName("order").HasDefaultValue(1);
            builder.Property(i => i.FechaRegistro).HasColumnName("FechaRegistro").HasColumnType("timestamp with time zone");
            builder.Property(i => i.Activa).HasColumnName("Activa").HasDefaultValue(true);
        }
    }
}