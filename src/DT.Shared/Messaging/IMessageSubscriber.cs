using DT.Shared.Interfaces;

namespace DT.Shared.Messaging;

public interface IMessageSubscriber
{
    Task SubscribeAsync<T>(
        string queue,
        IConsumer<T> handler,
        CancellationToken cancellationToken = default) where T : IMessage;
}