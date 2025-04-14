public interface IMessageBus 
{
    Task PublishAsync<TEvent>(TEvent @event) where TEvent : class;
    Task SubscribeAsync<TEvent>(Func<TEvent, Task> handler) where TEvent : class;
}
