using DT.Saga.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DT.Saga.Infrastructure.Database.Configurations;

public class SagaStateConfiguration : IEntityTypeConfiguration<SagaState>
{
    public void Configure(EntityTypeBuilder<SagaState> builder)
    {
        builder.ToTable("saga_states", "saga");

        builder.HasKey(s => s.CorrelationId);
        
        builder.Property(s => s.CorrelationId)
            .HasColumnName("correlation_id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(s => s.OrderId)
            .HasColumnName("order_id")
            .IsRequired();
        
        builder.Property(s => s.CurrentState)
            .HasColumnName("current_state")
            .HasMaxLength(50);
        
        builder.Property(s => s.SagaType)
            .HasColumnName("saga_type")
            .HasMaxLength(20);
        
        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");
        
        builder.HasIndex(s => new { s.SagaType, s.IsCompleted })
            .HasDatabaseName("idx_saga_states_type_completed");
        
        builder.HasIndex(s => s.OrderId)
            .HasDatabaseName("idx_saga_states_order_id");

    }
}