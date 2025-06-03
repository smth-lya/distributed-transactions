using DT.Orders.Domain.Contracts.Services;
using DT.Orders.Domain.Enums;
using DT.Shared.Commands.Order;
using DT.Shared.Events.Order;
using DT.Shared.Interfaces;
using DT.Shared.Messaging;

namespace DT.Orders.API.Consumers.Orchestration;

public class OrderCompleteConsumer : IConsumer<OrderCompleteCommand>, IHostedService
{
    private readonly IMessageSubscriber _subscriber;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<OrderCompleteConsumer> _logger;
    
    public OrderCompleteConsumer(IServiceScopeFactory scopeFactory, IMessageSubscriber subscriber, ILogger<OrderCompleteConsumer> logger)
    {
        _subscriber = subscriber;
        _serviceScopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCompleteCommand> context)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IOrderService>();
        
        await service.UpdateOrderStatusAsync(context.Message.OrderId, OrderStatus.Delivered, "Completed");
        
        _logger.LogInformation("[Order Service] [OrderCompleteConsumer] Success transaction ended: {Message}" ,new string('F', 50));
        
        await context.PublishAsync(
            new OrderCompletedEvent(),
            "saga.orchestration.events",
            string.Empty);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {   
        // TODO: Сделать более контролируемый порядок декларирования очередей и обменников перед их использованием
        await _subscriber.SubscribeAsync("order.saga.orchestration.commands", this, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}