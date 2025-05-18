namespace DT.Shared.Messaging;

public interface IMessagePublisher
{
    public Task PublishAsync<T>(
        T message,
        string exchange,
        string routingKey,
        string? correlationId,
        CancellationToken cancellationToken = default) where T : IMessage;
}