namespace DT.Common.Messaging;

public abstract record Message()
{
    public Guid MessageId { get; init; } = Guid.NewGuid();
    public Guid CorrelationId { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    
    // public string? SpanId { get; init; } // Для распределенного трейсинга (OpenTelemetry)
    // public string? Source { get; init; } // Источник сообщения
}