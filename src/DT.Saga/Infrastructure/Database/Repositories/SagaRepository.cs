using DT.Saga.Domain.Contracts.Repositories;
using DT.Saga.Domain.Enums;
using DT.Saga.Domain.Exceptions;
using DT.Saga.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DT.Saga.Infrastructure.Database.Repositories;

public class SagaRepository : ISagaRepository
{
    private readonly SagaDbContext _context;
    private readonly ILogger<SagaRepository> _logger;

    public SagaRepository(SagaDbContext context, ILogger<SagaRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<SagaState?> GetSagaState(Guid correlationId)
    {
        return await _context.States
            .Include(s => s.Events)
            .Include(s => s.Commands)
            .AsSplitQuery()
            .FirstOrDefaultAsync(s => s.CorrelationId == correlationId);
    }

    public async Task AddSagaAsync(SagaState saga)
    {
        var loadedSaga = await _context.States.FindAsync(saga.CorrelationId);
        if (loadedSaga is not null)
        {
            throw new SagaAlreadyExistsException(saga.CorrelationId);
        }
        
        _context.States.Add(saga);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateSagaAsync(SagaState saga)
    {
        var loadedSaga = await _context.States.FirstOrDefaultAsync(s => s.CorrelationId == saga.CorrelationId);
        if (loadedSaga is null)
        {
            throw new SagaNotFoundException(saga.CorrelationId);
        }
        
        saga.UpdatedAt = DateTime.UtcNow;
        
        _context.States.Update(saga);
        await _context.SaveChangesAsync();
    }

    public async Task AddEventAsync(SagaEvent @event)
    {
        _context.Events.Add(@event);
        await _context.SaveChangesAsync();
    }

    public async Task AddCommandAsync(SagaCommand command)
    {
        _context.Commands.Add(command);
        await _context.SaveChangesAsync();
    }

    public async Task MarkCommandAsCompletedAsync(Guid commandId)
    {
        var command = await _context.Commands.FindAsync(commandId);
        if (command != null)
        {
            command.Status = CommandStatus.Completed;
            command.SentAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task MarkCommandAsFailedAsync(Guid commandId)
    {
        var command = await _context.Commands.FindAsync(commandId);
        if (command != null)
        {
            command.Status = CommandStatus.Failed;
            await _context.SaveChangesAsync();
        }
    }
}