using DT.Shared.Messaging;

namespace DT.Shared.Interfaces;

public interface IConsumer<T> where T : IMessage
{
    Task Consume(ConsumeContext<T> context);
}


