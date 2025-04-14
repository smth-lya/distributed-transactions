using ApiGateway.Core;

namespace ApiGateway.Runtime;

public interface ISagaContextFactory
{
    ISagaContext Create(Guid correlationId, CancellationToken cancellationToken = default);
}