using DT.Orders.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DT.Orders.Infrastructure.Database.Configurations;

public class OrderStatusChangeConfiguration : IEntityTypeConfiguration<OrderStatusChange>
{
    public void Configure(EntityTypeBuilder<OrderStatusChange> builder)
    {
        builder.ToTable("order_status_changes");
        
        builder.HasKey(s => s.Id)
            .HasName("pk_order_status_changes");
        
        builder.Property(s => s.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");
        
        builder.Property(s => s.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.ChangeReason)
            .HasColumnName("change_reason")
            .HasMaxLength(500);

        builder.Property(s => s.ChangedAt)
            .HasColumnName("changed_at")
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'")
            .IsRequired();
        
        builder.HasIndex(s => s.OrderId)
            .HasDatabaseName("idx_status_changes_order_id");
        
        builder.HasIndex(s => s.ChangedAt)
            .HasDatabaseName("idx_status_changes_changed_at");
    }
}