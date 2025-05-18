using DT.Common.Commands;
using DT.Common.Events;
using DT.Common.Interfaces;
using DT.Common.Messaging;

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