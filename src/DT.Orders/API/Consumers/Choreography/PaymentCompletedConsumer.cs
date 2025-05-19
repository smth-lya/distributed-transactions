using DT.Shared.Events;
using DT.Shared.Interfaces;
using DT.Shared.Messaging;

namespace DT.Orders.API.Consumers.Choreography;

public class PaymentCompletedConsumer : IConsumer<PaymentCompletedEvent>
{
    public PaymentCompletedConsumer(IMessageSubscriber subscriber)
    {
        subscriber.SubscribeAsync("order.saga.choreography.events", this);
    }

    public Task Consume(ConsumeContext<PaymentCompletedEvent> context)
    {
        // End of transaction
        return Task.CompletedTask;
    }
}