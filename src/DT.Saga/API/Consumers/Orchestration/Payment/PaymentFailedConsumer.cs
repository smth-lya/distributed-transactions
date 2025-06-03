using System.Text.Json;
using DT.Saga.Domain.Contracts.Repositories;
using DT.Saga.Domain.Enums;
using DT.Saga.Domain.Models;
using DT.Shared.Commands.Inventory;
using DT.Shared.Events.Payment;
using DT.Shared.Interfaces;
using DT.Shared.Messaging;

namespace DT.Saga.API.Consumers.Orchestration.Payment;

public class PaymentFailedConsumer : IConsumer<PaymentFailedEvent>, IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMessageSubscriber _subscriber;
    private readonly ILogger<PaymentFailedConsumer> _logger;
    
    public PaymentFailedConsumer(
        IServiceScopeFactory scopeFactory, 
        IMessageSubscriber subscriber, 
        ILogger<PaymentFailedConsumer> logger)
    {
        _subscriber = subscriber;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
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
                .FirstOrDefault(e => e is { EventType: nameof(PaymentFailedEvent), IsProcessed: true });

            if (existingEvent?.IsProcessed == true)
            {
                _logger.LogWarning("Event already processed. Skipping");
                return;
            }
            
            _logger.LogInformation(
                "Processing payment failure for Order {OrderId}. Reason: {Reason}",
                saga.OrderId,
                context.Message.Reason);
        
            saga.CurrentState = nameof(PaymentFailedEvent);
            saga.UpdatedAt = DateTime.UtcNow;
            saga.IsCompleted = false;
            
            await repository.UpdateSagaAsync(saga);

            await repository.AddEventAsync(new SagaEvent()
            {
                Id = Guid.NewGuid(),
                CorrelationId = correlationId,
                EventType = nameof(PaymentFailedEvent),
                Payload = JsonSerializer.Serialize(context.Message),
                OccurredAt = DateTime.UtcNow,
                IsProcessed = true
            });
            
            var compensationCommand = new InventoryCancelReservationCommand(saga.OrderId);
            
            try
            {
                await context.PublishAsync(
                    compensationCommand,
                    "saga.orchestration.commands",
                    "inventory");

                await repository.AddCommandAsync(new SagaCommand
                {
                    Id = Guid.NewGuid(),
                    CorrelationId = correlationId,
                    CommandType = nameof(InventoryCancelReservationCommand),
                    Payload = JsonSerializer.Serialize(compensationCommand),
                    CreatedAt = DateTime.UtcNow,
                    SentAt = DateTime.UtcNow,
                    Status = CommandStatus.Pending
                });

                _logger.LogInformation(
                    "Compensation command sent for Order {OrderId}",
                    saga.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to publish compensation command for Order {OrderId}",
                    saga.OrderId);
                    
                // Помечаем команду как неудачную
                await repository.AddCommandAsync(new SagaCommand
                {
                    Id = Guid.NewGuid(),
                    CorrelationId = correlationId,
                    CommandType = nameof(InventoryCancelReservationCommand),
                    Payload = JsonSerializer.Serialize(compensationCommand),
                    CreatedAt = DateTime.UtcNow,
                    Status = CommandStatus.Failed
                });
                
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "Error processing PaymentFailedEvent for CorrelationId: {CorrelationId}", 
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