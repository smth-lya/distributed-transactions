namespace ApiGateway.Messaging;

public interface IMessageSubscriber
{
    Task SubscribeAsync<TEvent>(Func<TEvent, Task> handler) where TEvent : class;
}