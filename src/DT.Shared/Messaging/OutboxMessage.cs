using System.Text.Json;

namespace DT.Shared.Messaging;

public class OutboxMessage
{
    public Guid Id { get; init; }
    public required string MessageType { get; init; } 
    public required string Payload { get; init; }     
    public required DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; private set; }
    
    public int RetryCount { get; set; }
    
    public Guid CorrelationId { get; init; }
    public string Exchange { get; init; }
    public string RoutingKey { get; init; }
    
    private OutboxMessage() { }

    public static OutboxMessage Create<TMessage>(
        TMessage message, 
        string exchange,
        string routingKey,
        Guid correlationId) where TMessage : IMessage
    {
        return new OutboxMessage()
        {
            Id = Guid.NewGuid(),
            MessageType = typeof(TMessage).Name,
            Payload = JsonSerializer.Serialize(message),
            CreatedAt = DateTime.UtcNow,
            Exchange = exchange,
            RoutingKey = routingKey,
            CorrelationId = correlationId
        };
    }
    
    public void MarkAsProcessed() => ProcessedAt = DateTime.UtcNow;
}