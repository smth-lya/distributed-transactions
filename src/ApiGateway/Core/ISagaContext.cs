namespace ApiGateway.Core;

public interface ISagaContext
{
    Guid CorrelationId { get; }
    CancellationToken CancellationToken { get; }
    
    IMetadataStore Metadata { get; }
    IEventAwaiter EventAwaiter { get; }
    ITracingContext TracingContext { get; }
}