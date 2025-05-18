using DT.Shared.Commands;
using DT.Shared.Events;
using DT.Shared.Interfaces;
using DT.Shared.Messaging;

namespace DT.Orders.Consumers.Orchestration;

public class ApprovedOrderConsumer : IConsumer<ApproveOrderCommand>
{
    public ApprovedOrderConsumer(IMessageSubscriber subscriber)
    {
        subscriber.SubscribeAsync("order.saga.orchestration.commands", this);
    }

    public async Task Consume(ConsumeContext<ApproveOrderCommand> context)
    {
        Console.WriteLine(new string('F', 50));
        
        await context.PublishAsync(
            new OrderApprovedEvent(),
            "saga.orchestration.events",
            string.Empty);
    }
}