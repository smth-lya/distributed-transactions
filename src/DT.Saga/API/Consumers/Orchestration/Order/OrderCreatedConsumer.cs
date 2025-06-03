using System.Text.Json;
using DT.Saga.Domain.Contracts.Repositories;
using DT.Saga.Domain.Enums;
using DT.Saga.Domain.Models;
using DT.Shared.Commands.Inventory;
using DT.Shared.DTOs;
using DT.Shared.Events.Order;
using DT.Shared.Interfaces;
using DT.Shared.Messaging;

namespace DT.Saga.API.Consumers.Orchestration.Order;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>, IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMessageSubscriber _subscriber;
    private readonly ILogger<OrderCreatedConsumer> _logger;
    
    public OrderCreatedConsumer(
        IServiceScopeFactory scopeFactory, 
        IMessageSubscriber subscriber, 
        ILogger<OrderCreatedConsumer> logger)
    {
        _subscriber = subscriber;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ISagaRepository>();

        try
        {
            var loadedSaga = await repository.GetSagaState(context.CorrelationId);
            if (loadedSaga != null)
            {
                _logger.LogWarning("Saga {CorrelationId} already exists", 
                    context.CorrelationId);
                return;
            }
            
            var saga = new SagaState()
            {
                CorrelationId = context.CorrelationId,
                OrderId = context.Message.OrderId,
                SagaType = "OrderProcessing",
                CurrentState = nameof(OrderCreatedEvent),
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
            };
        
            await repository.AddSagaAsync(saga);
        
            await repository.AddEventAsync(new SagaEvent()
            {
                Id = Guid.NewGuid(),
                CorrelationId = context.CorrelationId,
                EventType = nameof(OrderCreatedEvent),
                Payload = JsonSerializer.Serialize(context.Message),
                OccurredAt = DateTime.UtcNow,
                IsProcessed = true
            });
            
            _logger.LogInformation("SagaState created for OrderId {OrderId}", context.Message.OrderId);
        
            var reserveCommand = new InventoryReserveCommand(
                context.Message.OrderId,
                context.Message.Items
                    .Select(i => new InventoryItemShared(i.ProductId, i.Quantity))
                    .ToList()
            );
        
            await repository.AddCommandAsync(new SagaCommand()
            {
                Id = Guid.NewGuid(),
                CorrelationId = context.CorrelationId,
                CommandType = nameof(InventoryReserveCommand),
                Payload = JsonSerializer.Serialize(reserveCommand),
                Status = CommandStatus.Pending,
                CreatedAt = DateTime.UtcNow,
            });
            
            await context.PublishAsync(
                reserveCommand,
                "saga.orchestration.commands",
                "inventory");


        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while processing OrderCreatedEvent for OrderId: {OrderId}", context.Message.OrderId);
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