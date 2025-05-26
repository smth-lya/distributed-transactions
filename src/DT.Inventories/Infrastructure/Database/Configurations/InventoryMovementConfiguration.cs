using DT.Inventories.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DT.Inventories.Infrastructure.Database.Configurations;

public class InventoryMovementConfiguration : IEntityTypeConfiguration<InventoryMovement>
{
    public void Configure(EntityTypeBuilder<InventoryMovement> builder)
    {
        builder.ToTable("inventory_movements");
        
        builder.HasKey(m => m.Id)
            .HasName("pk_inventory_movements");
        
        builder.Property(m => m.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");
        
        builder.Property(m => m.MovementType)
            .HasColumnName("movement_type")
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(m => m.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");
        
        builder.HasIndex(m => new { m.ProductId, m.CreatedAt })
            .HasDatabaseName("idx_movements_product_created");
        
        builder.HasOne(m => m.Product)
            .WithMany(m => m.Movements)
            .HasForeignKey(m => m.ProductId)
            .HasConstraintName("fk_movements_products")
            .OnDelete(DeleteBehavior.Restrict);
    }
}