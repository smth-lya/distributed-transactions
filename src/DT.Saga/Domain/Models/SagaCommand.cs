using DT.Saga.Domain.Enums;

namespace DT.Saga.Domain.Models;

public class SagaCommand
{
    public required Guid Id { get; init; }
    public required Guid CorrelationId { get; init; }
    public required string CommandType { get; init; }
    public required string Payload { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? SentAt { get; set; }
    public required CommandStatus Status { get; set; }
    
    public SagaState Saga { get; init; }
}