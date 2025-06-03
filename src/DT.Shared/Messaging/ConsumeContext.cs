using Microsoft.Extensions.Logging;

namespace DT.Shared.Messaging;

public class ConsumeContext<TMessage> where TMessage : IMessage
{
    public required TMessage Message { get; init; }
    public required Guid CorrelationId { get; init; }
    
    public IMessagePublisher Publisher { get; init; }
    
    public async Task PublishAsync<T>(
        T message,
        string exchange,
        string routingKey,
        CancellationToken cancellationToken = default) where T : IMessage
    {
        await Publisher.PublishAsync(message, exchange, routingKey, CorrelationId, cancellationToken);
    }
    
    //public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    
    // public string? SpanId { get; init; } // Для распределенного трейсинга (OpenTelemetry)
    // public string? Source { get; init; } // Источник сообщения
}