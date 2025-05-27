using DT.Orders.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DT.Orders.Infrastructure.Database.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");
        
        builder.HasKey(o => o.Id)
            .HasName("pk_orders");
        
        builder.Property(o => o.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");
        
        builder.Property(o => o.CustomerId)
            .HasColumnName("customer_id")
            .IsRequired();
        
        builder.Property(o => o.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // builder.Property(o => o.M)
        //     .IsRowVersion()
        //     .IsConcurrencyToken();
        
        builder.Property(o => o.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'")
            .ValueGeneratedOnAdd();

        builder.Property(o => o.UpdatedAt)
            .HasColumnName("updated_at");
        
        builder.OwnsOne(o => o.ShippingAddress, sa =>
        {
            sa.Property(a => a.Street).HasColumnName("shipping_street").HasMaxLength(100);
            sa.Property(a => a.City).HasColumnName("shipping_city").HasMaxLength(50);
            sa.Property(a => a.State).HasColumnName("shipping_state").HasMaxLength(50);
            sa.Property(a => a.Country).HasColumnName("shipping_country").HasMaxLength(50);
            sa.Property(a => a.ZipCode).HasColumnName("shipping_zipcode").HasMaxLength(20);
        });
        
        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(i => i.OrderId)
            .HasConstraintName("fk_order_items_orders")
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(o => o.StatusChanges)
            .WithOne()
            .HasForeignKey(s => s.OrderId)
            .HasConstraintName("fk_status_changes_orders")
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(o => o.CustomerId)
            .HasDatabaseName("idx_orders_customer_id");
        
        builder.HasIndex(o => o.Status)
            .HasDatabaseName("idx_orders_status");
    }
}