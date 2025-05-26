using DT.Inventories.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DT.Inventories.Infrastructure.Database.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("reservations");
        
        builder.HasKey(r => r.Id)
            .HasName("pk_reservation");
        
        builder.Property(r => r.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");
        
        builder.Property(r => r.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(r => r.ReservedAt)
            .HasColumnName("reserved_at")
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");
        
        builder.HasIndex(r => r.OrderId)
            .HasDatabaseName("idx_reservation_order_id");
        
        builder.ToTable(t => t.HasCheckConstraint("ck_reservations_quantity_positive", "\"quantity\" >= 0\""));
        
        builder.HasOne(r => r.Product)
            .WithMany(p => p.Reservations)
            .HasForeignKey(r => r.ProductId)
            .HasConstraintName("fk_reservation_products")
            .OnDelete(DeleteBehavior.Restrict);
    }
}