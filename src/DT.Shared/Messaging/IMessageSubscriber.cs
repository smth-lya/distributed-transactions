using DT.Common.Interfaces;

namespace DT.Common.Messaging;

public interface IMessageSubscriber
{
    Task SubscribeAsync<T>(
        string queue,
        IConsumer<T> handler,
        CancellationToken cancellationToken = default) where T : IMessage;
}