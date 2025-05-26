namespace DT.Saga.Domain.Models;

public class SagaEvent
{
    public required Guid Id { get; init; }
    public required Guid CorrelationId { get; init; }
    public required string EventType { get; init; }
    public required string Payload { get; init; }
    public required DateTime OccurredAt { get; init; }
    public bool IsProcessed { get; set; }
    
    public SagaState Saga { get; init; }
}