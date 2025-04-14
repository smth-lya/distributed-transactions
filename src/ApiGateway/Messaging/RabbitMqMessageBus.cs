
namespace ApiGateway.Messaging;

public class RabbitMqMessageBus : IMessageBus
{
    public Task SubscribeAsync<TEvent>(Func<TEvent, Task> handler) where TEvent : class
    {
        throw new NotImplementedException();
    }

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct = default) where TEvent : class
    {
        throw new NotImplementedException();
    }
}
