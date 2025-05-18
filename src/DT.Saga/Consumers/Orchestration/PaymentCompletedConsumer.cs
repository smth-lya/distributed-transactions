using DT.Common.Commands;
using DT.Common.DTOs;
using DT.Common.Events;
using DT.Common.Interfaces;
using DT.Common.Messaging;

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