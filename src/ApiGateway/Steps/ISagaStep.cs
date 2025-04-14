using ApiGateway.Core;

public interface ISagaStep<TState> where TState : ISagaState
{
    Task ExecuteAsync(TState state, ISagaContext context);
    Task CompensateAsync(TState state, ISagaContext context);
    bool CanCompensate(TState state) => true;
}
