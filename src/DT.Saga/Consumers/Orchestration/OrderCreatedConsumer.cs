using DT.Shared.Commands;
using DT.Shared.DTOs;
using DT.Shared.Events;
using DT.Shared.Interfaces;
using DT.Shared.Messaging;

namespace DT.Saga.Consumers.Orchestration;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    public OrderCreatedConsumer(IMessageSubscriber subscriber)
    {
        subscriber.SubscribeAsync("saga.orchestration.events", this);
    }
    
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        await context.PublishAsync(
            new ReserveInventoryCommand(context.Message.Items.Select(i => new InventoryItemShared(i.ProductId, i.Quantity)).ToList()),
            "saga.orchestration.commands",
            "inventory");
    }
}