using DT.Common.Commands;
using DT.Common.Events;
using DT.Common.Interfaces;
using DT.Common.Messaging;

namespace DT.Payments.Consumers.Orchestration;

public class ProcessPaymentConsumer : IConsumer<ProcessPaymentCommand>
{
    public ProcessPaymentConsumer(IMessageSubscriber subscriber)
    {
        subscriber.SubscribeAsync("payment.saga.orchestration.commands", this);
    }

    public async Task Consume(ConsumeContext<ProcessPaymentCommand> context)
    {
        await context.PublishAsync(
            new PaymentCompletedEvent(context.Message.Amount),
            "saga.orchestration.events",
            string.Empty);
    }
}