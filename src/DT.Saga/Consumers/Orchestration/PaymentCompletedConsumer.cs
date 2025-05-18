using DT.Shared.Commands;
using DT.Shared.DTOs;
using DT.Shared.Events;
using DT.Shared.Interfaces;
using DT.Shared.Messaging;

namespace DT.Saga.Consumers.Orchestration;

public class PaymentCompletedConsumer : IConsumer<PaymentCompletedEvent>
{
    public PaymentCompletedConsumer(IMessageSubscriber subscriber)
    {
        subscriber.SubscribeAsync("saga.orchestration.events", this);
    }

    public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
    {
        await context.PublishAsync(
            new ApproveOrderCommand(),
            "saga.orchestration.commands",
            "order");
    }
}