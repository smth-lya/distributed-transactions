using System.Text.Json;
using DT.Saga.Domain.Contracts.Repositories;
using DT.Saga.Domain.Enums;
using DT.Saga.Domain.Models;
using DT.Shared.Commands.Payment;
using DT.Shared.Events.Inventory;
using DT.Shared.Events.Order;
using DT.Shared.Interfaces;
using DT.Shared.Messaging;

namespace DT.Saga.API.Consumers.Orchestration.Inventory;

public class InventoryReservedConsumer : IConsumer<InventoryReservedEvent>, IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMessageSubscriber _subscriber;
    private readonly ILogger<InventoryReservedConsumer> _logger;
    
    public InventoryReservedConsumer(
        IServiceScopeFactory scopeFactory, 
        IMessageSubscriber subscriber, 
        ILogger<InventoryReservedConsumer> logger)
    {
        _subscriber = subscriber;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<InventoryReservedEvent> context)
    {
        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ISagaRepository>();

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
                .FirstOrDefault(e => e is { EventType: nameof(InventoryReservedEvent), IsProcessed: true });

            if (existingEvent?.IsProcessed == true)
            {
                _logger.LogWarning("Event already processed. Skipping.");
                return;
            }
            
            var orderCreatedEvent  = saga.Events
                .FirstOrDefault(e => e is { EventType: nameof(OrderCreatedEvent), IsProcessed: true });
           
            if (orderCreatedEvent  == null)
            {
                _logger.LogError("OrderCreatedEvent not found for saga {CorrelationId}", context.CorrelationId);
                return;
            }
            
            var orderDetails = JsonSerializer.Deserialize<OrderCreatedEvent>(orderCreatedEvent.Payload);
            if (orderDetails == null)
            {
                _logger.LogError("Failed to deserialize OrderCreatedEvent for saga {CorrelationId}", context.CorrelationId);
                return;
            }
            
            saga.CurrentState = nameof(InventoryReservedEvent);
            saga.UpdatedAt = DateTime.UtcNow;
            saga.IsCompleted = false; 
            
            await repository.UpdateSagaAsync(saga);
            
            await repository.AddEventAsync(new SagaEvent()
            {
                Id = Guid.NewGuid(),
                CorrelationId = context.CorrelationId,
                EventType = nameof(InventoryReservedEvent),
                Payload = JsonSerializer.Serialize(context.Message),
                OccurredAt = DateTime.UtcNow,
                IsProcessed = true
            });
            
            var paymentAmount = orderDetails.Items.Sum(item => item.Price * item.Quantity);
            var paymentCommand = new PaymentProcessCommand(saga.OrderId, paymentAmount, "USD");
            
            await context.PublishAsync(
                paymentCommand,
                "saga.orchestration.commands",
                "payment");

            await repository.AddCommandAsync(new SagaCommand()
            {
                Id = Guid.NewGuid(),
                CorrelationId = context.CorrelationId,
                CommandType = nameof(PaymentProcessCommand),
                Payload = JsonSerializer.Serialize(paymentCommand),
                Status = CommandStatus.Pending,
                CreatedAt = DateTime.UtcNow,
            });
            
            _logger.LogInformation(
                "Successfully processed InventoryReservedEvent for order {OrderId}. Payment command sent for amount {Amount}",
                orderDetails.OrderId,
                paymentAmount);
        }
        catch (JsonException jsonEx)
        {
            _logger.LogError(jsonEx, "JSON deserialization error while processing InventoryReservedEvent");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while processing InventoryReservedEvent for CorrelationId: {CorrelationId}", 
                context.CorrelationId);
            throw;
        }
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _subscriber.SubscribeAsync("saga.orchestrator.events", this, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}