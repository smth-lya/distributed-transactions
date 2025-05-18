using DT.Common.Messaging;

namespace DT.Common.Interfaces;

public interface IConsumer<T> where T : IMessage
{
    Task Consume(ConsumeContext<T> context);
}