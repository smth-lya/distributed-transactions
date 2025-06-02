using Microsoft.Extensions.Logging;

namespace DT.Shared.Messaging;

public class ConsumeContext<TMessage> where TMessage : IMessage
{
    private readonly IBrokerPublisher _brokerPublisher;
    private readonly IOutboxPublisher _outboxPublisher;
    
    public ConsumeContext(IBrokerPublisher brokerPublisher, IOutboxPublisher outboxPublisher)
    {
        _brokerPublisher = brokerPublisher;
        _outboxPublisher = outboxPublisher;
    }
    
    public required TMessage Message { get; init; }
    public required Guid CorrelationId { get; init; }
    
    public bool UseOutbox { get; init; }
    
    public async Task PublishAsync<T>(
        T message,
        string exchange,
        string routingKey,
        CancellationToken cancellationToken = default) where T : IMessage
    {
        if (UseOutbox)
            await _outboxPublisher.PublishAsync(message, exchange, routingKey, CorrelationId, cancellationToken);
        else
            await _brokerPublisher.PublishAsync(message, exchange, routingKey, CorrelationId, cancellationToken);
    }
    
    //public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    
    // public string? SpanId { get; init; } // Для распределенного трейсинга (OpenTelemetry)
    // public string? Source { get; init; } // Источник сообщения
}