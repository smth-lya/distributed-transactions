using System.Text.Json;
using DT.Saga.Domain.Contracts.Repositories;
using DT.Saga.Domain.Enums;
using DT.Saga.Domain.Models;
using DT.Shared.Commands.Inventory;
using DT.Shared.Commands.Order;
using DT.Shared.Commands.Payment;
using DT.Shared.Events.Payment;
using DT.Shared.Interfaces;
using DT.Shared.Messaging;

namespace DT.Saga.API.Consumers.Orchestration.Payment;

public class PaymentRefundConsumer : IConsumer<PaymentRefundEvent>, IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMessageSubscriber _subscriber;
    private readonly ILogger<PaymentRefundConsumer> _logger;
    
    public PaymentRefundConsumer(
        IServiceScopeFactory scopeFactory, 
        IMessageSubscriber subscriber, 
        ILogger<PaymentRefundConsumer> logger)
    {
        _subscriber = subscriber;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentRefundEvent> context)
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
                .FirstOrDefault(e => e is { EventType: nameof(PaymentRefundEvent), IsProcessed: true });

            if (existingEvent?.IsProcessed == true)
            {
                _logger.LogWarning("Event already processed. Skipping.");
                return;
            }

            saga.CurrentState = nameof(PaymentRefundEvent);
            saga.UpdatedAt = DateTime.UtcNow;
            saga.IsCompleted = false; 
            
            await repository.UpdateSagaAsync(saga);
            
            await repository.AddEventAsync(new SagaEvent()
            {
                Id = Guid.NewGuid(),
                CorrelationId = context.CorrelationId,
                EventType = nameof(PaymentRefundEvent),
                Payload = JsonSerializer.Serialize(context.Message),
                OccurredAt = DateTime.UtcNow,
                IsProcessed = true
            });
            
            var orderCommand = new InventoryCancelReservationCommand(saga.OrderId);
            
            await context.PublishAsync(
                orderCommand,
                "saga.orchestration.commands",
                "inventory");

            await repository.AddCommandAsync(new SagaCommand()
            {
                Id = Guid.NewGuid(),
                CorrelationId = context.CorrelationId,
                CommandType = nameof(InventoryCancelReservationCommand),
                Payload = JsonSerializer.Serialize(orderCommand),
                Status = CommandStatus.Compensated,
                CreatedAt = DateTime.UtcNow,
            });
            
            _logger.LogInformation(
                "Successfully processed PaymentRefundEvent for order {OrderId}. Order completed command sent",
                saga.OrderId
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while processing PaymentRefundEvent for CorrelationId: {CorrelationId}", 
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