public class SagaContext : ISagaContext
{
    public Guid CorrelationId { get; }
    public CancellationToken CancellationToken { get; }
    public Dictionary<string, object> Metadata { get; } = new();

    public SagaContext(Guid correlationId, CancellationToken ct = default)
    {
        CorrelationId = correlationId;
        CancellationToken = ct;
    }
}
