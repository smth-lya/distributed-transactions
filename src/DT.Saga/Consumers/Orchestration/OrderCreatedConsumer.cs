using DT.Common.Commands;
using DT.Common.DTOs;
using DT.Common.Events;
using DT.Common.Interfaces;
using DT.Common.Messaging;

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
            new ReserveInventoryCommand([new InventoryItem(context.Message.ProductId, context.Message.Quantity)]),
            "saga.orchestration.commands",
            "inventory");
    }
}