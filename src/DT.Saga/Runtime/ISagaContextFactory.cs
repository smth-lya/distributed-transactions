using DT.Saga.Core;

namespace DT.Saga.Runtime;

public interface ISagaContextFactory
{
    ISagaContext Create(Guid correlationId, CancellationToken cancellationToken = default);
}