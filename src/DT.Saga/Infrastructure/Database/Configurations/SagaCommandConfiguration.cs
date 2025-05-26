using DT.Saga.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DT.Saga.Infrastructure.Database.Configurations;

public class SagaCommandConfiguration : IEntityTypeConfiguration<SagaCommand>
{
    public void Configure(EntityTypeBuilder<SagaCommand> builder)
    {
        builder.ToTable("saga_commands", "saga");
        
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(c => c.CommandType)
            .HasColumnName("command_type")
            .HasMaxLength(100);
        
        builder.Property(c => c.Payload)
            .HasColumnName("command_type")
            .HasMaxLength(500);
        
        builder.Property(c => c.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20);
        
        builder.HasOne(c => c.Saga)
            .WithMany(s => s.Commands)
            .HasForeignKey(c => c.CorrelationId)
            .HasConstraintName("fk_saga_commands_states")
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(c => new { c.CorrelationId, c.Status })
            .HasDatabaseName("idx_saga_commands_correlation_status");
    }
}