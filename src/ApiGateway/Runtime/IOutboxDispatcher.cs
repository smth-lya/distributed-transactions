namespace ApiGateway.Runtime;

public interface IOutboxDispatcher
{
    Task FlushAsync(Guid correlationId, CancellationToken cancellationToken = default);
}