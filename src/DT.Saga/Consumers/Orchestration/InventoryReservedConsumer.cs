using DT.Common.Commands;
using DT.Common.DTOs;
using DT.Common.Events;
using DT.Common.Interfaces;
using DT.Common.Messaging;

namespace DT.Saga.Consumers.Orchestration;

public class InventoryReservedConsumer : IConsumer<InventoryReservedEvent>
{
    public InventoryReservedConsumer(IMessageSubscriber subscriber)
    {
        subscriber.SubscribeAsync("saga.orchestration.events", this);
    }

    public async Task Consume(ConsumeContext<InventoryReservedEvent> context)
    {
        await context.PublishAsync(
            new ProcessPaymentCommand(10, "USD", 0),
            "saga.orchestration.commands",
            "payment");
    }
}