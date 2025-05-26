using DT.Saga.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DT.Saga.Infrastructure.Database.Configurations;

public class SagaEventConfiguration : IEntityTypeConfiguration<SagaEvent>
{
    public void Configure(EntityTypeBuilder<SagaEvent> builder)
    {
        builder.ToTable("saga_events", "saga");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(e => e.EventType)
            .HasColumnName("event_type")
            .HasMaxLength(100);
        
        builder.Property(e => e.Payload)
            .HasColumnName("payload")
            .HasMaxLength(500);
        
        builder.HasOne(e => e.Saga)
            .WithMany(s => s.Events)
            .HasForeignKey(e => e.CorrelationId)
            .HasConstraintName("fk_saga_events_states")
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(e => new { e.CorrelationId, e.IsProcessed })
            .HasDatabaseName("idx_saga_events_correlation_processed");
    }
}