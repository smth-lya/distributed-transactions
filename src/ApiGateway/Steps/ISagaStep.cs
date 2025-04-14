using ApiGateway.Core;

public interface ISagaStep<TState> where TState : ISagaState
{
    string Name { get; }
    Task ExecuteAsync(TState state, ISagaContext context);
    Task CompensateAsync(TState state, ISagaContext context);
    bool CanCompensate(TState state) => true;
}
