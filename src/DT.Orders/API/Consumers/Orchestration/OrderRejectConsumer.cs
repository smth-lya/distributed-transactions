using DT.Orders.Domain.Contracts.Services;
using DT.Orders.Domain.Enums;
using DT.Shared.Commands.Order;
using DT.Shared.Events.Order;
using DT.Shared.Interfaces;
using DT.Shared.Messaging;

namespace DT.Orders.API.Consumers.Orchestration;

public class OrderRejectConsumer : IConsumer<OrderRejectCommand>, IHostedService
{
    private readonly IMessageSubscriber _subscriber;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<OrderRejectConsumer> _logger;
    
    public OrderRejectConsumer(IServiceScopeFactory scopeFactory, IMessageSubscriber subscriber, ILogger<OrderRejectConsumer> logger)
    {
        _subscriber = subscriber;
        _serviceScopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderRejectCommand> context)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IOrderService>();

        try
        {
            await service.UpdateOrderStatusAsync(context.Message.OrderId, OrderStatus.Cancelled, context.Message.Reason);
        
            await context.PublishAsync(
                new OrderRejectedEvent("Order Rejected"),
                "saga.orchestration.events",
                string.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while processing OrderRejectCommand for CorrelationId: {CorrelationId}", context.CorrelationId);
            throw;
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _subscriber.SubscribeAsync("order.saga.orchestration.commands", this, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}