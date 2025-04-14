using ApiGateway.Core;
using ApiGateway.Runtime;

public class SagaRunner<TState> : ISagaRunner<TState> 
    where TState : ISagaState
{
    private readonly ISagaRepository<TState> _repository;
    private readonly IMessageBus _bus;
    private readonly IRetryPolicy _retryPolicy;
    private readonly ILogger<SagaRunner<TState>> _logger;

    public SagaRunner(
        ISagaRepository<TState> repository,
        IMessageBus bus,
        IRetryPolicy retryPolicy,
        ILogger<SagaRunner<TState>> logger)
    {
        _repository = repository;
        _bus = bus;
        _retryPolicy = retryPolicy;
        _logger = logger;
    }

    public async Task RunAsync(
        Guid correlationId, 
        IEnumerable<ISagaStep<TState>> steps,
        CancellationToken ct = default) 
    {
        var state = await _repository.LoadAsync(correlationId);
        var context = new SagaContext(correlationId, ct);

        try
        {
            state.Status = SagaStatus.InProgress;
            await _repository.SaveAsync(state);

            foreach (var step in steps)
            {
                await ExecuteStepWithRetry(step, state, context);
                await _repository.SaveAsync(state);
            }

            state.Status = SagaStatus.Completed;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, $"Saga {correlationId} failed");

            await CompensateAsync(state, steps.Reverse(), context);
            await _bus.PublishAsync(new SagaFailedEvent(correlationId, ex));
        }
        finally
        {
            await _repository.SaveAsync(state);
        }
    }

    private async Task ExecuteStepWithRetry(
        ISagaStep<TState> step,
        TState state,
        ISagaContext context)
    {
        await _retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                await step.ExecuteAsync(state, context);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    $"Step {step.GetType().Name} failed for saga {state.CorrelationId}");
                throw;
            }
        });
    }


    private async Task CompensateAsync(
        TState state,
        IEnumerable<ISagaStep<TState>> steps,
        ISagaContext context)
    {
        foreach (var step in steps)
        {
            try
            {
                await _retryPolicy.ExecuteAsync(
                    () => step.CompensateAsync(state, context));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Compensation failed for step {StepName} in saga {CorrelationId}",
                    step.GetType().Name, state.CorrelationId);
            }
        }

        state.Status = SagaStatus.Compensated;
    }

    public Task RunAsync(TState state, IEnumerable<ISagaStep<TState>> steps, ISagaContext context, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
