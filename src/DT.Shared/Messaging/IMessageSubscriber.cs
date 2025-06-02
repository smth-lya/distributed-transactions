using DT.Shared.Interfaces;

namespace DT.Shared.Messaging;

public interface IMessageSubscriber
{
    Task SubscribeAsync<T>(
        string exchangeName,
        IConsumer<T> handler,
        bool useOutbox = true,
        CancellationToken cancellationToken = default) where T : IMessage;
}