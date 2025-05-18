using DT.Shared.Commands;
using DT.Shared.Events;
using DT.Shared.Interfaces;
using DT.Shared.Messaging;

namespace DT.Inventories.Consumers.Orchestration;

public class ReserveInventoryConsumer : IConsumer<ReserveInventoryCommand>
{
    public ReserveInventoryConsumer(IMessageSubscriber subscriber)
    {
        subscriber.SubscribeAsync("inventory.saga.orchestration.commands", this);
    }
    public async Task Consume(ConsumeContext<ReserveInventoryCommand> context)
    {
        await context.PublishAsync(
            new InventoryReservedEvent(context.Message.Items),
            "saga.orchestration.events",
            string.Empty);
    }
}