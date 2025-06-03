using System.Text.Json;
using DT.Saga.Domain.Contracts.Repositories;
using DT.Saga.Domain.Enums;
using DT.Saga.Domain.Models;
using DT.Saga.Infrastructure.Database;
using DT.Shared.Events.Order;
using DT.Shared.Interfaces;
using DT.Shared.Messaging;

namespace DT.Saga.API.Consumers.Orchestration.Order;

public class OrderCompletedConsumer : IConsumer<OrderCompletedEvent>, IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMessageSubscriber _subscriber;
    private readonly ILogger<OrderCompletedConsumer> _logger;
    
    public OrderCompletedConsumer(
        IServiceScopeFactory scopeFactory, 
        IMessageSubscriber subscriber, 
        ILogger<OrderCompletedConsumer> logger)
    {
        _subscriber = subscriber;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<OrderCompletedEvent> context)
    {
        using var scope = _scopeFactory.CreateScope();
        
        var dbContext = scope.ServiceProvider.GetRequiredService<SagaDbContext>();
        var repository = scope.ServiceProvider.GetRequiredService<ISagaRepository>();

        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        
        try
        {
            var saga = await repository.GetSagaState(context.CorrelationId);
            if (saga == null || saga.IsCompleted)
            {
                _logger.LogWarning("Saga {CorrelationId} not found or already completed", 
                    context.CorrelationId);
                return;
            }
            
            var existingEvent = saga.Events
                .FirstOrDefault(e => e is { EventType: nameof(OrderCompletedEvent), IsProcessed: true });

            if (existingEvent?.IsProcessed == true)
            {
                _logger.LogWarning("Event already processed. Skipping.");
                return;
            }

            saga.CurrentState = nameof(OrderCompletedEvent);
            saga.IsCompleted = true;
            saga.UpdatedAt = DateTime.UtcNow;

            await repository.AddEventAsync(new SagaEvent()
            {
                Id = Guid.NewGuid(),
                CorrelationId = context.CorrelationId,
                EventType = nameof(OrderCompletedEvent),
                Payload = JsonSerializer.Serialize(context.Message),
                OccurredAt = DateTime.UtcNow,
                IsProcessed = true
            });

            foreach (var cmd in saga.Commands.Where(c => c.Status == CommandStatus.Pending))
            {
                cmd.Status = CommandStatus.Completed;
                cmd.SentAt = DateTime.UtcNow;
            }

            await repository.UpdateSagaAsync(saga);
            await transaction.CommitAsync();
            
            _logger.LogInformation("Saga {CorrelationId} successfully completed", 
                context.CorrelationId);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Failed to complete saga {CorrelationId}", 
                context.CorrelationId);
            throw;
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _subscriber.SubscribeAsync("saga.orchestration.events", this, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}