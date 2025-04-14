using ApiGateway.Core;

namespace ApiGateway.Runtime;

public interface ISagaRunner<TState> where TState : ISagaState
{
    Task RunAsync(
        TState state,
        IEnumerable<ISagaStep<TState>> steps,
        ISagaContext context,
        CancellationToken ct = default);
}