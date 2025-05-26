using DT.Saga.Domain.Models;

namespace DT.Saga.Domain.Contracts.Repositories;

public interface ISagaRepository
{
    Task<SagaState?> GetSagaState(Guid correlationId);
    Task AddSagaAsync(SagaState saga);
    Task UpdateSagaAsync(SagaState saga);
    
    Task AddEventAsync(SagaEvent @event);
    Task AddCommandAsync(SagaCommand command);
    
    Task MarkCommandAsCompletedAsync(Guid commandId);
    Task MarkCommandAsFailedAsync(Guid commandId);
}