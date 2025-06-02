using System.Text.Json;
using DT.Saga.Domain.Contracts.Repositories;
using DT.Saga.Domain.Enums;
using DT.Saga.Domain.Models;
using DT.Shared.Commands.Order;
using DT.Shared.Events.Inventory;
using DT.Shared.Interfaces;
using DT.Shared.Messaging;

namespace DT.Saga.API.Consumers.Orchestration.Inventory;

public class InventoryReservationFailedConsumer : IConsumer<InventoryReservationFailedEvent>, IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMessageSubscriber _subscriber;
    private readonly ILogger<InventoryReservationFailedConsumer> _logger;
    
    public InventoryReservationFailedConsumer(
        IServiceScopeFactory scopeFactory, 
        IMessageSubscriber subscriber, 
        ILogger<InventoryReservationFailedConsumer> logger)
    {
        _subscriber = subscriber;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<InventoryReservationFailedEvent> context)
    {
        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ISagaRepository>();

        try
        {
            var correlationId = context.CorrelationId;
            var saga = await repository.GetSagaState(correlationId);

            if (saga == null)
            {
                _logger.LogWarning("Saga with CorrelationId {CorrelationId} not found for inventory reservation failure", correlationId);
                return;
            }
        
            var existingEvent = saga.Events
                .FirstOrDefault(e => e is { EventType: nameof(InventoryReservationFailedEvent), IsProcessed: true });

            if (existingEvent?.IsProcessed == true)
            {
                _logger.LogWarning("Event already processed. Skipping");
                return;
            }
            
            _logger.LogInformation("Inventory reservation failed for Saga {CorrelationId}. Rejecting order {OrderId}. Reason: {Reason}",
                correlationId, saga.OrderId, context.Message.Reason);
        
            saga.CurrentState = nameof(InventoryReservationFailedEvent);
            saga.UpdatedAt = DateTime.UtcNow;
            saga.IsCompleted = false;
            
            await repository.UpdateSagaAsync(saga);
            
            var sagaEvent = new SagaEvent()
            {
                Id = Guid.NewGuid(),
                CorrelationId = correlationId,
                EventType = nameof(InventoryReservationFailedEvent),
                Payload = JsonSerializer.Serialize(context.Message),
                OccurredAt = DateTime.UtcNow,
                IsProcessed = true
            };
            
            await repository.AddEventAsync(sagaEvent);
            
            var rejectCommand = new OrderRejectCommand(saga.OrderId, context.Message.Reason);
            
            await context.PublishAsync(
                rejectCommand,
                "saga.orchestration.commands",
                "order");

            var sagaCommand = new SagaCommand()
            {
                Id = Guid.NewGuid(),
                CorrelationId = correlationId,
                CommandType = nameof(OrderRejectCommand),
                Payload = JsonSerializer.Serialize(rejectCommand),
                CreatedAt = DateTime.UtcNow,
                Status = CommandStatus.Compensated
            };
            
            await repository.AddCommandAsync(sagaCommand);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while processing InventoryReservationFailedEvent for CorrelationId: {CorrelationId}", context.CorrelationId);
            throw;
        }
       
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _subscriber.SubscribeAsync("saga.orchestration.events", this, true, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}