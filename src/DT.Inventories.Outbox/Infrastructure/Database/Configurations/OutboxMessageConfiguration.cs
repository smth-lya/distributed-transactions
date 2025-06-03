using DT.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DT.Inventories.Outbox.Infrastructure.Database.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages", "messaging");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever(); 
        
        builder.Property(x => x.MessageType)
            .HasColumnName("message_type")
            .HasMaxLength(255)
            .IsRequired();
            
        builder.Property(x => x.Payload)
            .HasColumnName("payload")
            .HasColumnType("jsonb") 
            .IsRequired();
            
        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
            
        builder.Property(x => x.ProcessedAt)
            .HasColumnName("processed_at")
            .IsRequired(false);
            
        builder.Property(x => x.RetryCount)
            .HasColumnName("retry_count")
            .HasDefaultValue(0);
            
        builder.Property(x => x.CorrelationId)
            .HasColumnName("correlation_id")
            .IsRequired();
            
        builder.Property(x => x.Exchange)
            .HasColumnName("exchange")
            .HasMaxLength(255)
            .IsRequired();
            
        builder.Property(x => x.RoutingKey)
            .HasColumnName("routing_key")
            .HasMaxLength(255)
            .IsRequired();
        
        
        builder.HasIndex(x => x.CorrelationId)
            .HasDatabaseName("ix_outbox_messages_correlation_id");
            
        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("ix_outbox_messages_created_at");
            
        builder.HasIndex(x => x.ProcessedAt)
            .HasDatabaseName("ix_outbox_messages_processed_at")
            .HasFilter("processed_at IS NULL"); // Частичный индекс для непрочитанных сообщений
            
        builder.HasIndex(x => x.MessageType)
            .HasDatabaseName("ix_outbox_messages_message_type");
            
        
        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("now() at time zone 'utc'");
    }
}