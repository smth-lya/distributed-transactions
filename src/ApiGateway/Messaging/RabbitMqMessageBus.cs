
namespace ApiGateway.Messaging;

public class RabbitMqMessageBus : IMessageBus
{
    public Task PublishAsync<TEvent>(TEvent @event) where TEvent : class
    {
        throw new NotImplementedException();
    }

    public Task SubscribeAsync<TEvent>(Func<TEvent, Task> handler) where TEvent : class
    {
        throw new NotImplementedException();
    }
}
