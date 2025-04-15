namespace ApiGateway.Core;

public interface ISagaContext
{
    Guid CorrelationId { get; }
    string SagaName { get; }
    string StepName { get; }
    
    CancellationToken CancellationToken { get; }
    
    IMetadataStore Metadata { get; }
    IEventAwaiter EventAwaiter { get; }
    ITracingContext TracingContext { get; }
    
    // Поддержка идемпотентности
    bool IsStepCompleted(string stepName);
    void MarkStepCompleted(string stepName);    
}