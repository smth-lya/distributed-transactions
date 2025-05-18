using DT.Shared.DTOs;
using DT.Shared.Events;
using DT.Shared.Interfaces;
using DT.Shared.Messaging;

namespace DT.Inventories.Consumers.Choreography;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    public OrderCreatedConsumer(IMessageSubscriber subscriber)
    {
        subscriber.SubscribeAsync("inventory.saga.choreography.events", this);
    }
    
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        await context.PublishAsync(
            new InventoryReservedEvent([new InventoryItem(context.Message.ProductId, context.Message.Quantity)]),
            "saga.choreography.events",
            string.Empty);
    }
}