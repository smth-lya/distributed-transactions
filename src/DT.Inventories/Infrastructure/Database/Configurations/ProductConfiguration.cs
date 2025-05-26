using DT.Inventories.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DT.Inventories.Infrastructure.Database.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");
        
        builder.HasKey(p => p.Id)
            .HasName("pk_products");
        
        builder.Property(p => p.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");
        
        builder.Property(p => p.SKU)
            .HasColumnName("sku")
            .HasMaxLength(50)
            .IsRequired()
            .IsUnicode(false);
        
        builder.HasIndex(p => p.SKU)
            .IsUnique()
            .HasDatabaseName("idx_products_sku");

        builder.Property(p => p.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");
        
        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone");
        
        builder.HasMany(p => p.InventoryItems)
            .WithOne(ii => ii.Product)
            .HasForeignKey(ii => ii.ProductId)
            .HasConstraintName("fk_inventory_items_products")
            .OnDelete(DeleteBehavior.Restrict);
    }
}