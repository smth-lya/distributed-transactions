using DT.Orders.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DT.Orders.Infrastructure.Database.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items", 
            t =>
            {
                t.HasCheckConstraint("ck_order_items_quantity_positive", "quantity > 0");
                t.HasCheckConstraint("ck_order_items_unit_price_positive", "unit_price > 0");
            });
        
        builder.HasKey(oi => oi.Id)
            .HasName("pk_order_items");
        
        builder.Property(i => i.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");
        
        builder.Property(oi => oi.ProductId)
            .HasColumnName("product_id")
            .IsRequired();
        
        builder.Property(oi => oi.ProductName)
            .HasColumnName("product_name")
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(oi => oi.UnitPrice)
            .HasColumnName("unit_price")
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.Property(oi => oi.Quantity)
            .HasColumnName("quantity")
            .IsRequired();
    }
}