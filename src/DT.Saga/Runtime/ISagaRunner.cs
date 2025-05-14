using DT.Saga.Core;
using DT.Saga.Steps;

namespace DT.Saga.Runtime;

public interface ISagaRunner<TState> where TState : ISagaState
{
    Task RunAsync(
        TState state,
        IEnumerable<ISagaStep<TState>> steps,
        ISagaContext context,
        CancellationToken cancellationToken = default);
}