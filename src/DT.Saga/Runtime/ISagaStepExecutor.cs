using DT.Saga.Core;
using DT.Saga.Steps;

namespace DT.Saga.Runtime;

public interface ISagaStepExecutor<TState> where TState : ISagaState
{
    Task ExecuteAsync(ISagaStep<TState> step, TState state, ISagaContext context, CancellationToken cancellationToken = default);
    Task CompensateAsync(ISagaStep<TState> step, TState state, ISagaContext context, CancellationToken cancellationToken = default);
}

// ? Насколько норм практика во все Async методы пихать CancellationToken.
// Особенно в микросервисной архитектуре ?