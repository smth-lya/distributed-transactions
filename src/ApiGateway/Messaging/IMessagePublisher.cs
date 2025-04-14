namespace ApiGateway.Messaging;

public interface IMessagePublisher
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct = default) where TEvent : class;
}