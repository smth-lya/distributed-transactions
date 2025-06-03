using DT.Inventories.Domain.Contracts.Repositories;
using DT.Inventories.Domain.Contracts.Services;
using DT.Shared.Commands;
using DT.Shared.Commands.Inventory;
using DT.Shared.Events;
using DT.Shared.Events.Inventory;
using DT.Shared.Interfaces;
using DT.Shared.Messaging;

namespace DT.Inventories.API.Consumers.Orchestration;

public class InventoryCancelReservationConsumer : IConsumer<InventoryCancelReservationCommand>, IHostedService
{
    private readonly IMessageSubscriber _subscriber;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<InventoryCancelReservationConsumer> _logger;
    
    public InventoryCancelReservationConsumer(IServiceScopeFactory scopeFactory, IMessageSubscriber subscriber, ILogger<InventoryCancelReservationConsumer> logger)
    {
        _subscriber = subscriber;
        _serviceScopeFactory = scopeFactory;
        _logger = logger;
    }
    public async Task Consume(ConsumeContext<InventoryCancelReservationCommand> context)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IInventoryService>();

        try
        {
            var released = await service.ReleaseItemsAsync(context.Message.OrderId);
            
            if (!released)
            {
                _logger.LogError("Failed to release items for reservation {OrderId}", context.Message.OrderId);
                return;
            }
        
            _logger.LogInformation("Inventory cancel reservation successfully. CorrelationId: {CorrelationId}", context.CorrelationId);

            var canceledEvent = new InventoryReservationCanceledEvent(""); 
            
            await context.PublishAsync(
                canceledEvent,
                "saga.orchestration.events",
                string.Empty
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while cancellation inventory. CorrelationId: {CorrelationId}", context.CorrelationId);
            throw;
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _subscriber.SubscribeAsync("inventory.saga.orchestration.commands", this, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}