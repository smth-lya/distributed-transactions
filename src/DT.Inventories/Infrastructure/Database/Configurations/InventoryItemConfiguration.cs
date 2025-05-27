using DT.Inventories.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DT.Inventories.Infrastructure.Database.Configurations;

public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.ToTable("inventory_items", t =>
        {
            t.HasCheckConstraint("ck_inventory_items_quantity_non_negative", "quantity >= 0");
            t.HasCheckConstraint("ck_inventory_items_reserved_lte_quantity", "reserved >= quantity");
        });
        
        builder.HasKey(ii => ii.Id)
            .HasName("pk_inventory_items");
        
        builder.Property(ii => ii.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");
        
        builder.HasIndex(ii => new { ii.ProductId, ii.WarehouseId })
            .IsUnique()
            .HasDatabaseName("idx_inventory_items_product_warehouse");
        
        builder.Property(ii => ii.Quantity)
            .HasColumnName("quantity")
            .HasDefaultValue("0");
        
        builder.Property(ii => ii.Reserved)
            .HasColumnName("reserved")
            .HasDefaultValue("0");
    }
}