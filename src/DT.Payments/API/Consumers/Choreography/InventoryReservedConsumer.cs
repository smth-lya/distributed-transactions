using DT.Shared.Events;
using DT.Shared.Events.Inventory;
using DT.Shared.Events.Payment;
using DT.Shared.Interfaces;
using DT.Shared.Messaging;

namespace DT.Payments.API.Consumers.Choreography;

public class InventoryReservedConsumer : IConsumer<InventoryReservedEvent>
{
    public InventoryReservedConsumer(IMessageSubscriber subscriber)
    {
        subscriber.SubscribeAsync("payment.saga.choreography.events", this);
    }

    public async Task Consume(ConsumeContext<InventoryReservedEvent> context)
    {
        await context.PublishAsync(
            new PaymentCompletedEvent(10),
            "saga.choreography.events",
            string.Empty);
    }
}