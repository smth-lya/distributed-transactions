using DT.Shared.DTOs;
using DT.Shared.Events;
using DT.Shared.Events.Inventory;
using DT.Shared.Events.Order;
using DT.Shared.Interfaces;
using DT.Shared.Messaging;

namespace DT.Inventories.API.Consumers.Choreography;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    public OrderCreatedConsumer(IMessageSubscriber subscriber)
    {
        subscriber.SubscribeAsync("inventory.saga.choreography.events", this);
    }
    
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        await context.PublishAsync(
            new InventoryReservedEvent(
                context.Message.Items.Select(i => new InventoryItemShared(i.ProductId, i.Quantity)).ToList()),
            "saga.choreography.events",
            string.Empty);
    }
}