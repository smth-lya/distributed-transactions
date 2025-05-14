namespace DT.Saga;

public interface ISagaState
{
    Guid CorrelationId { get; set; }
    SagaStatus Status { get; set; }
    DateTimeOffset CreatedAt { get; set; }
    Dictionary<string, object> ContextData { set; }
}