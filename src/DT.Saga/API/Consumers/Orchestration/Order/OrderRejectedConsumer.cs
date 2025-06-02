using System.Text.Json;
using DT.Saga.Domain.Contracts.Repositories;
using DT.Saga.Domain.Models;
using DT.Shared.Events.Order;
using DT.Shared.Interfaces;
using DT.Shared.Messaging;

namespace DT.Saga.API.Consumers.Orchestration.Order;

public class OrderRejectedConsumer : IConsumer<OrderRejectedEvent>, IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMessageSubscriber _subscriber;
    private readonly ILogger<OrderRejectedConsumer> _logger;
    
    public OrderRejectedConsumer(
        IServiceScopeFactory scopeFactory, 
        IMessageSubscriber subscriber, 
        ILogger<OrderRejectedConsumer> logger)
    {
        _subscriber = subscriber;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<OrderRejectedEvent> context)
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
                .FirstOrDefault(e => e is { EventType: nameof(OrderRejectedEvent), IsProcessed: true });

            if (existingEvent?.IsProcessed == true)
            {
                _logger.LogWarning("Event already processed. Skipping.");
                return;
            }
            
            saga.CurrentState = nameof(OrderCompletedEvent);
            saga.UpdatedAt = DateTime.UtcNow;
            // При успешной компенсации транзакции помечаем сагу как завершенную.
            saga.IsCompleted = true;           

            await repository.UpdateSagaAsync(saga);
            
            await repository.AddEventAsync(new SagaEvent()
            {
                Id = Guid.NewGuid(),
                CorrelationId = context.CorrelationId,
                EventType = nameof(OrderRejectedEvent),
                Payload = JsonSerializer.Serialize(context.Message),
                OccurredAt = DateTime.UtcNow,
                IsProcessed = true
            });

            _logger.LogInformation(
                "Saga {CorrelationId} completed. Order {OrderId} rejected. Reason: {Reason}",
                context.CorrelationId,
                saga.OrderId,
                context.Message.Reason
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while processing OrderRejectedEvent for CorrelationId: {CorrelationId}", context.CorrelationId);
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