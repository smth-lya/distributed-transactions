using ApiGateway.Core;
using ApiGateway.Runtime;

public class SagaRunner<TState> : ISagaRunner<TState> 
    where TState : ISagaState
{
    private readonly ISagaRepository<TState> _repository;
    private readonly ISagaStepExecutor<TState> _stepExecutor;
    private readonly ISagaContextFactory _contextFactory;
    private readonly ILogger<SagaRunner<TState>> _logger;

    public SagaRunner(
        ISagaRepository<TState> repository,
        ISagaStepExecutor<TState> stepExecutor,
        ISagaContextFactory contextFactory,
        ILogger<SagaRunner<TState>> logger)
    {
        _repository = repository;
        _stepExecutor = stepExecutor;
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task RunAsync(
        TState state, 
        IEnumerable<ISagaStep<TState>> steps, 
        ISagaContext context, 
        CancellationToken cancellationToken = default) 
    {
        try
        {
            state.Status = SagaStatus.InProgress;
            await _repository.SaveAsync(state, cancellationToken);

            foreach (var step in steps)
            {
                await _stepExecutor.ExecuteAsync(step, state, context, cancellationToken);
                context.MarkStepCompleted(step.Name);
                await _repository.SaveAsync(state, cancellationToken);
            }

            state.Status = SagaStatus.Completed;
            await _repository.SaveAsync(state, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, $"Saga {state.CorrelationId} failed, starting compensation");
            await CompensateAsync(state, steps.Reverse(), context, cancellationToken);

            state.Status = SagaStatus.Compensated;
            await _repository.SaveAsync(state, cancellationToken);            
        }
    }

    private async Task CompensateAsync(
        TState state,
        IEnumerable<ISagaStep<TState>> steps,
        ISagaContext context,
        CancellationToken cancellationToken = default)
    {
        foreach (var step in steps)
        {
            if (!context.IsStepCompleted(step.Name))
                continue;

            try
            {
                await _stepExecutor.CompensateAsync(step, state, context, cancellationToken);
                _logger.LogInformation("Compensated step {Step}", step.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Compensation failed for step {Step}", step.Name);
            }
        }
    }
}
