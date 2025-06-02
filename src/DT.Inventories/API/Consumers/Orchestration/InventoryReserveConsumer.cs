using DT.Inventories.Domain.Contracts.Repositories;
using DT.Inventories.Domain.Contracts.Services;
using DT.Shared.Commands;
using DT.Shared.Commands.Inventory;
using DT.Shared.Events;
using DT.Shared.Events.Inventory;
using DT.Shared.Interfaces;
using DT.Shared.Messaging;

namespace DT.Inventories.API.Consumers.Orchestration;

public class InventoryReserveConsumer : IConsumer<InventoryReserveCommand>, IHostedService
{
    private readonly IMessageSubscriber _subscriber;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<InventoryReserveConsumer> _logger;
    
    public InventoryReserveConsumer(
        IServiceScopeFactory scopeFactory, 
        IMessageSubscriber subscriber, 
        ILogger<InventoryReserveConsumer> logger)
    {
        _subscriber = subscriber;
        _serviceScopeFactory = scopeFactory;
        _logger = logger;
    }
    public async Task Consume(ConsumeContext<InventoryReserveCommand> context)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IInventoryService>();

        try
        {
            var reservationSuccess = await service.ReserveItemAsync(
                context.Message.OrderId,
                context.Message.Items
                    .ToDictionary(k => k.ProductId, v => v.Quantity)
            );

            if (!reservationSuccess)
            {
                _logger.LogWarning("Inventory reservation failed. CorrelationId: {CorrelationId}", context.CorrelationId);
                
                await context.PublishAsync(
                    new InventoryReservationFailedEvent(context.Message.Items, "Unable to reserve one or more items."),
                    "saga.orchestration.events",
                    string.Empty
                );
            
                return;
            }
        
            _logger.LogInformation("Inventory reserved successfully. CorrelationId: {CorrelationId}", context.CorrelationId);

            var reservedEvent = new InventoryReservedEvent(context.Message.Items); 
            
            await context.PublishAsync(
                reservedEvent,
                "saga.orchestration.events",
                string.Empty
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reserving inventory. CorrelationId: {CorrelationId}", context.CorrelationId);
            
            await context.PublishAsync(
                new InventoryReservationFailedEvent(context.Message.Items, "Unable to reserve one or more items."),
                "saga.orchestration.events",
                string.Empty
            );
        }

    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _subscriber.SubscribeAsync("inventory.saga.orchestration.commands", this, true, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}